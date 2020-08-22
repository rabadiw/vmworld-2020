using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stuuck.Data;
using Stuuck.Models;
using System;
using System.Linq;

namespace Stuuck.Api
{
    public static class EFCoreDataExtensions
    {
        internal static IHost InitializeDbContexts(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    InitializeStuuckContexts(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<WebHostBuilder>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            return host;
        }

        internal static void InitializeStuuckContexts(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<StuuckContext>();
                db.Database.EnsureCreated();
            }
            InitializeContext(serviceProvider);
        }

        private static void InitializeContext(IServiceProvider serviceProvider)
        {
            using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var db = serviceScope.ServiceProvider.GetService<StuuckContext>();
            if (!db.Reminders.Any())
            {
                db.Reminders.AddRange(GetReminders());
            }

            if (!db.ReminderSchedules.Any())
            {
                db.ReminderSchedules.AddRange(GetReminderSchedules());
            }

            db.SaveChanges();
        }

        private static ReminderSchedule[] GetReminderSchedules()
        {
            return new[] {
                new ReminderSchedule { Id = 1, ReminderId = 1, Schedule = DateTime.Parse("2020-08-06T01:00:00-05:00") },
                new ReminderSchedule { Id = 2, ReminderId = 1, Schedule = DateTime.Parse("2020-08-08T01:00:00-05:00") }
            };
        }

        private static Reminder[] GetReminders()
        {
            return new[]{
                new Reminder { Id = 1, Title = "Read Eric Evans book on DDD" },
                new Reminder { Id = 2, Title = "Learn about .NET 5" },
                new Reminder { Id = 3, Title = "Remember to eat lunch!" }
            };
        }

        //private static void AddData<TData>(this DbContext db, object item) where TData : class
        //{
        //    db.Entry(item).State = EntityState.Added;
        //}
    }
}
