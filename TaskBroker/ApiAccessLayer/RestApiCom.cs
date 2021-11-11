using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Newtonsoft.Json;
using TaskBroker.Models;


namespace TaskBroker.ApiAccessLayer
{
    public class RestApiCom : IRestApiCom
    {
        private readonly IRestClient _restClient;

        public RestApiCom(IServiceScopeFactory serviceScopeFactory)
        {
            _restClient = 
                serviceScopeFactory
                    .CreateScope()
                    .ServiceProvider
                    .GetRequiredService<IRestClient>();

            ServicePointManager.ServerCertificateValidationCallback
                += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public async Task<RowColMultiplicationDto> SendTaskToAgent(dynamic body, string url)
        {
            var request = new RestRequest(url);
            request.AddJsonBody(body);

            var response = await _restClient.ExecutePostAsync(request);

            var result =
                JsonConvert.DeserializeObject<RowColMultiplicationDto>(response.Content);

            return result;
        }

    }
}
