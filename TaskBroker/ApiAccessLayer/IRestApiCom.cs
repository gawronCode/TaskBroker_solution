using System.Threading.Tasks;
using TaskBroker.Models;

namespace TaskBroker.ApiAccessLayer
{
    public interface IRestApiCom
    {
        Task<RowColMultiplicationDto> SendTaskToAgent(dynamic body, string url);
    }
}