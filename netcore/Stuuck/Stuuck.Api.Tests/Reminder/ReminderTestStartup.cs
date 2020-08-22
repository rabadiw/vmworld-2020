using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Stuuck.Api.Tests.Reminder
{
    internal class ReminderTestStartup : Startup
    {
        public ReminderTestStartup(IConfiguration env) : base(env)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services
                .AddMvcCore()
                .AddApplicationPart(typeof(Startup).Assembly);
        }
    }
}
