using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Extensions.Logging;
using System;
using System.Web.Hosting;
using System.Web.Http;

namespace Stuuck
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // configure json formatter
            var jsonformatter = config.Formatters.JsonFormatter;
            jsonformatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        // TODO: enable dynamic configuration loading
        public static IConfigurationBuilder CreateConfigurationBuilder()
        {
            var env = new
            {
                ContentRootPath = HostingEnvironment.ApplicationPhysicalPath,
                EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
            };

            return
                new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    // TODO: local development support
                    // TODO: all upper level environment should use environment variables
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables()
                    // TODO: add TAS support using Steeltoe
                    .AddCloudFoundry();
        }

        // TODO: enable dynamic logging
        public static (ILoggerProvider, ILoggerFactory) CreateLoggerFactory(IConfiguration configuration)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddConfiguration(configuration)
                    .AddDynamicConsole();
            });
            return (
                serviceCollection.BuildServiceProvider().GetService<ILoggerProvider>(),
                serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>()
                );
        }
    }
}
