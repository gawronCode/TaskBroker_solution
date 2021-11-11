using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using TaskBroker.ApiAccessLayer;
using TaskBroker.ConfigData;
using TaskBroker.Services;


namespace TaskBroker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IConfigGetter, ConfigGetter>();
            services.AddScoped<IRestApiCom, RestApiCom>();
            services.AddScoped<ITaskBrokerService, TaskBrokerService>();
            services.AddScoped<IRestClient, RestClient>();

            services.AddControllers();
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
