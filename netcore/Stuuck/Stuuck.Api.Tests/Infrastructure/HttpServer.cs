using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace Stuuck.Testing
{
    public interface ITestServerBuilder
    {
        IWebHostBuilder WebHostBuilder { get; }

        TestServer Build();
    }

    public class TestServerBuilder : ITestServerBuilder
    {
        public TestServerBuilder(IWebHostBuilder webHostBuilder)
        {
            WebHostBuilder = webHostBuilder;
        }

        public IWebHostBuilder WebHostBuilder { get; }

        public TestServer Build() => new TestServer(WebHostBuilder);
    }
}
namespace Stuuck.Extensions.Testing
{
    public static class TestServerBuilder
    {
        public static Stuuck.Testing.ITestServerBuilder CreateDefaultBuilder(string envName)
        {
            return new Stuuck.Testing.TestServerBuilder(
                new WebHostBuilder()
                    .ConfigureAppConfiguration(builder => builder
                        .AddJsonFile($"{envName}/appsettings.json", optional: false)
                        .AddEnvironmentVariables()));
        }

        public static Stuuck.Testing.ITestServerBuilder UseContentRoot<TType>(this Stuuck.Testing.ITestServerBuilder serverBuilder)
        {
            var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TType)).Location);
            serverBuilder.WebHostBuilder.UseContentRoot(path);

            return serverBuilder;
        }

        public static Stuuck.Testing.ITestServerBuilder UseStartup<TStartup>(this Stuuck.Testing.ITestServerBuilder serverBuilder) where TStartup : class
        {
            serverBuilder.WebHostBuilder.UseStartup<TStartup>();
            return serverBuilder;
        }

        public static Stuuck.Testing.ITestServerBuilder ConfigureAppConfiguration(
            this Stuuck.Testing.ITestServerBuilder serverBuilder,
            Action<IConfigurationBuilder> configureDelegate)
        {

            serverBuilder.WebHostBuilder.ConfigureAppConfiguration(configureDelegate);
            return serverBuilder;
        }

        public static TestServer InitializeDbContexts<TContext>(this TestServer testServer, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {

            using var scope = testServer.Host.Services.CreateScope();

            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();

            try
            {
                logger.LogInformation("{MethodName}:: database associated with context {DbContextName}", nameof(InitializeDbContexts), typeof(TContext).Name);

                var retry = Policy.Handle<SqlException>()
                     .WaitAndRetry(new TimeSpan[]
                     {
                             TimeSpan.FromSeconds(3),
                             TimeSpan.FromSeconds(5),
                             TimeSpan.FromSeconds(8),
                     });

                retry.Execute(() => InvokeSeeder(seeder, context, services));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
            }

            return testServer;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            try
            {
                context.Database.Migrate();
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlex)
            {
                // DO NOTHING
            }

            seeder(context, services);
        }

        public static HttpClient CreateIdempotentClient(this TestServer server)
        {
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());
            return client;
        }

        public static HttpClient Accept(this HttpClient httpClient, string mediaType)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaType));
            return httpClient;
        }
    }
}