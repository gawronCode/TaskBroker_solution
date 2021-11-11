using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TaskBroker.ApiAccessLayer;
using TaskBroker.ConfigData;
using TaskBroker.Models;

namespace TaskBroker.Services
{
    public class TaskBrokerService : ITaskBrokerService
    {

        private readonly IConfigGetter _configGetter;
        private readonly IRestApiCom _restApiCom;

        private readonly List<RowColMultiplicationDto> _agentsResults = new();
        private readonly List<RowColMultiplicationDto> _agentsAssignments = new();
        private string[] _agentsIps;
        private Semaphore[] _agentsSemaphores;

        private readonly SemaphoreSlim _resultSem = new(1);
        

        public TaskBrokerService(IConfigGetter configGetter,
                                 IRestApiCom restApiCom)
        {
            _configGetter = configGetter;
            _restApiCom = restApiCom;
        }


        public async Task<MatrixMultiplicationDto> MultiplyMatricesAsync(
            MatrixMultiplicationDto matrixMultiplicationDto)
        {

            _agentsIps = _configGetter
                .GetMatrixAgents().AgentsHosts;

            _agentsSemaphores = _agentsIps.Select(a => new Semaphore(1, 1)).ToArray();

            Console.WriteLine("Tworzę zadania dla agentów");
            CreateAssignments(matrixMultiplicationDto.MatrixA, matrixMultiplicationDto.MatrixB);
            Console.WriteLine("Rozpoczynam zlecanie zadań dla agentów");

            var dataFlowOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _agentsIps.Length, 
                BoundedCapacity = -1
            };

            var block = new ActionBlock<RowColMultiplicationDto>(async assignment =>
            {
                var agentIndex = WaitHandle.WaitAny(_agentsSemaphores as WaitHandle[]);

                var url = _agentsIps[agentIndex] + @"/api/MatrixAgent/CalculateScalar";
                Console.WriteLine($"Zadanie zlecone agentowi na port {_agentsIps[agentIndex]}");
                var result 
                    = await _restApiCom.SendTaskToAgent(assignment, url);

                _agentsSemaphores[agentIndex].Release();

                await _resultSem.WaitAsync();

                Console.WriteLine($"Agent z portu {_agentsIps[agentIndex]} zwrócił wynik");
                _agentsResults.Add(result);
                _resultSem.Release();

            },dataFlowOptions);

            foreach (var assignment in _agentsAssignments)
                await block.SendAsync(assignment);
            
            

            block.Complete();
            await block.Completion;

            matrixMultiplicationDto.MatrixAxB = MatrixBuilder(_agentsResults);

            return matrixMultiplicationDto;
        }

        private void CreateAssignments(MatrixDto matrixA, MatrixDto matrixB)
        {
            var aMatrixRowCount = matrixA.ColumnsRows.First().Count();
            var aMatrixColCount = matrixA.ColumnsRows.Count();

            var aMatrixVectors = new List<List<double>>();
            var bMatrixVectors = new List<List<double>>();

            for (var rowIndex = 0; rowIndex < aMatrixRowCount; rowIndex++)
            {
                var aRowVector = new List<double>();
                var bColVector = new List<double>();
                for (var colIndex = 0; colIndex < aMatrixColCount; colIndex++)
                {
                    aRowVector.Add(matrixA.ColumnsRows.ToArray()[colIndex].ToArray()[rowIndex]);
                    bColVector.Add(matrixB.ColumnsRows.ToArray()[rowIndex].ToArray()[colIndex]);
                }
                aMatrixVectors.Add(aRowVector);
                bMatrixVectors.Add(bColVector);
            }

            var scalarPositionRow = 0;

            foreach (var aRowVector in aMatrixVectors)
            {
                var scalarPositionCol = 0;
                foreach (var bColVector in bMatrixVectors)
                {
                    _agentsAssignments.Add(new RowColMultiplicationDto
                    {
                        RowIndex = scalarPositionRow,
                        ColIndex = scalarPositionCol,
                        VectorRowA = aRowVector,
                        VectorColB = bColVector
                    });
                    scalarPositionCol++;
                }
                scalarPositionRow++;
            }
        }

        private MatrixDto MatrixBuilder(IEnumerable<RowColMultiplicationDto> rowCol)
        {
            var rowsCount = rowCol.Select(rc => rc.RowIndex).Max();
            var colsCount = rowCol.Select(rc => rc.ColIndex).Max();

            var columnsRows = new List<List<double>>();

            for (var colIndex = 0; colIndex <= colsCount; colIndex++)
            {
                var column = new List<double>();
                for (var rowIndex = 0; rowIndex <= rowsCount; rowIndex++)
                {
                    column.Add(rowCol.FirstOrDefault(rc =>
                        rc.ColIndex == colIndex && rc.RowIndex == rowIndex).ResultScalar);
                }
                columnsRows.Add(column);
            }

            return new MatrixDto {ColumnsRows = columnsRows};
        }

    }
}
