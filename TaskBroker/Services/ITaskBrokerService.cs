using System.Threading.Tasks;
using TaskBroker.Models;

namespace TaskBroker.Services
{
    public interface ITaskBrokerService
    {
        public Task<MatrixMultiplicationDto> MultiplyMatricesAsync(
            MatrixMultiplicationDto matrixMultiplicationDto);
    }
}