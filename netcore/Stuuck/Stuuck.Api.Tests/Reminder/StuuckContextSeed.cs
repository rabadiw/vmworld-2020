using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Stuuck.Data;
using Stuuck.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stuuck.Api.Tests.Reminder
{
    internal class StuuckContextSeeder
    {
        private readonly StuuckContext dbcontext;
        private readonly IServiceProvider services;

        public StuuckContextSeeder(StuuckContext dbcontext, IServiceProvider services)
        {
            this.dbcontext = dbcontext;
            this.services = services;
        }

        public async Task SeedAsync()
        {
            var logger = this.services.GetService<ILogger<StuuckContextSeeder>>();

            var policy = CreatePolicy(logger, nameof(StuuckContextSeeder));

            await policy.ExecuteAsync(async () =>
            {

                using (dbcontext)
                {
                    if (!dbcontext.Reminders.Any())
                    {
                        dbcontext.Reminders.AddRange(GetReminders());

                        await dbcontext.SaveChangesAsync();
                    }

                    if (!dbcontext.ReminderSchedules.Any())
                    {
                        dbcontext.ReminderSchedules.AddRange(GetReminderSchedules());

                        await dbcontext.SaveChangesAsync();
                    }
                }
            });
        }

        private ReminderSchedule[] GetReminderSchedules()
        {
            return new[] {
                new ReminderSchedule { Id = 1, ReminderId = 1, Schedule = DateTime.Parse("2020-08-06T01:00:00-05:00") },
                new ReminderSchedule { Id = 2, ReminderId = 1, Schedule = DateTime.Parse("2020-08-08T01:00:00-05:00") }
            };
        }

        private Models.Reminder[] GetReminders()
        {
            return new[]{
                new Models.Reminder { Id = 1, Title = "Read Eric Evans book on DDD" },
                new Models.Reminder { Id = 2, Title = "Learn about .NET 5" },
                new Models.Reminder { Id = 3, Title = "Remember to eat lunch!" }
            };
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<StuuckContextSeeder> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}