using Microsoft.Extensions.Configuration;


namespace TaskBroker.ConfigData
{
    public class ConfigGetter : IConfigGetter
    {
        private readonly IConfiguration _configuration;

        public ConfigGetter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MatrixAgentsOptions GetMatrixAgents()
        {
            return _configuration.GetSection(MatrixAgentsOptions.MatrixAgents).Get<MatrixAgentsOptions>();
        }
    }
}
