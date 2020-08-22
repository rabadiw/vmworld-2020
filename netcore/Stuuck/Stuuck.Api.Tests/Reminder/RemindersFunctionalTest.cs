using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stuuck.Data;
using Stuuck.Extensions.Testing;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Stuuck.Api.Tests.Reminder
{
    [TestClass]
    public class RemindersFunctionalTest
    {
        private TestServer httpServer;

        [TestInitialize]
        public void Setup()
        {
            httpServer =
                TestServerBuilder
                    .CreateDefaultBuilder("Reminder")
                    .UseContentRoot<RemindersFunctionalTest>()
                    .UseStartup<ReminderTestStartup>()
                    .Build()
                    .InitializeDbContexts<StuuckContext>(
                        async (dbContext, services) =>
                        {
                            await new StuuckContextSeeder(dbContext, services).SeedAsync();
                        });
        }

        [TestCleanup]
        public void Cleanup()
        {
            httpServer?.Dispose();
        }

        [TestMethod]
        public async Task GetReminders_Should_200_3_count()
        {
            var client = httpServer
                .CreateIdempotentClient()
                .Accept("application/json");

            var actual = await client.GetFromJsonAsync<List<Models.Reminder>>("api/reminders");
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count() == 3);
        }
    }
}
