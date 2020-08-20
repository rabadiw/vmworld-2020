using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Web.Http;

namespace Stuuck
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static ILoggerFactory LoggerFactory { get; set; }
        public static ILoggerProvider LoggerProvider { get; set; }

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // GOAL: enable containerization
            // GOAL: store config in the environment 
            // GOAL: logs as event stream, stdout, allow platform to persist logs

            // ISSUE: Use Web.config. Can't do this, ConfigurationManager is read only
            // Configuration.GetSection("ConnectionStrings").GetChildren().ToList().ForEach(conn =>
            //            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings(conn.Key, conn.Value)));

            // TODO: Enable dynamic configuration
            // TODO: Use .NET Standard abstraction libraries
            Configuration = WebApiConfig.CreateConfigurationBuilder().Build();

            // TODO: Enable container friendly logging
            // TODO: Use steeltoe dynamic logging
            (LoggerProvider, LoggerFactory) = WebApiConfig.CreateLoggerFactory(Configuration);

            // TODO: improve monitoring and management of the app
            // TODO: use steeltoe cloud management (actuators)
            ManagementConfig.ConfigureActuators(
                Configuration,
                LoggerProvider,
                GlobalConfiguration.Configuration.Services.GetApiExplorer(),
                LoggerFactory);
            ManagementConfig.Start();

            // TODO: test the output of logs
            LoggerFactory.CreateLogger<WebApiApplication>().LogInformation("Stuuck legacy web api started");
        }
    }
}
