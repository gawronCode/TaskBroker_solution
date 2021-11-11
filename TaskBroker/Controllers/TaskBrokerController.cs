using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskBroker.Models;
using TaskBroker.Services;

namespace TaskBroker.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskBrokerController : ControllerBase
    {
        private readonly ITaskBrokerService _taskBrokerService;

        public TaskBrokerController(ITaskBrokerService taskBrokerService)
        {
            _taskBrokerService = taskBrokerService;
        }

        [HttpPost]
        public async Task<ActionResult<MatrixMultiplicationDto>> MultiplyMatricesAxB(
            MatrixMultiplicationDto matrixMultiplicationDto)
        {
            Console.WriteLine("Odebrałem zadanie od klienta");

            var result = await _taskBrokerService.MultiplyMatricesAsync(matrixMultiplicationDto);
            
            Console.WriteLine("Zwracam wynik klientowi");
            
            return Ok(result);
        }

    }
}
