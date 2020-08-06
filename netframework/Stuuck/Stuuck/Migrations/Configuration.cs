namespace Stuuck.Migrations
{
    using Stuuck.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Stuuck.Data.StuuckContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Stuuck.Data.StuuckContext";
        }

        protected override void Seed(Stuuck.Data.StuuckContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            context.Reminders.AddOrUpdate(x => x.Id,
                new Reminder() { Id = 1, Title = "Read Eric Evans book on DDD" },
                new Reminder()
                {
                    Id = 2,
                    Title = "Learn about .NET 5",
                    Schedules = new System.Collections.Generic.List<ReminderSchedule>()
                    {
                        new ReminderSchedule(){ ReminderId=1, Schedule = DateTime.Parse("2020-08-06T01:00:00-05:00") },
                        new ReminderSchedule(){ ReminderId=1, Schedule = DateTime.Parse("2020-08-08T01:00:00-05:00") }
                    }
                },
                new Reminder() { Id = 3, Title = "Remember to eat lunch!" }
                );
        }
    }
}
