using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stuuck.Models;

namespace Stuuck.Tests
{
    [TestClass]
    public class ReminderUnitTest
    {
        // TODO: Unit Test Reminders
        // So, how do I start? 😕 
        // I want to make sure my code handles the CRUD correctly 
        // I want to make sure my REST endpoints are good

        // TODO: https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking

        [TestMethod]
        public void TestReminders()
        {
            //Arrange

            var configRoot = new Mock<IConfigurationRoot>();
            configRoot.Setup(config => config.GetSection("ConnectionStrings")["StuuckContext"]).Returns("testing");
            WebApiApplication.Configuration = configRoot.Object;

            var reminderSchedules = new List<Models.ReminderSchedule>
            {
                new ReminderSchedule{ Id = 1, ReminderId = 1, Schedule = DateTime.Parse("2020-08-06T01:00:00-05:00") }
            }.AsQueryable();
            var reminders = new List<Models.Reminder>
            {
                new Reminder{ Id = 1, Title = "Testing", Description = "", Schedules = reminderSchedules.ToList() }
            }.AsQueryable();

            var mockReminderSet = new Mock<DbSet<Models.Reminder>>();
            var mockScheduleSet = new Mock<DbSet<Models.ReminderSchedule>>();
            var mockDbContext = new Mock<Data.StuuckContext>();

            mockReminderSet.As<IDbAsyncEnumerable<Models.Reminder>>()
                .Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Models.Reminder>(reminders.GetEnumerator()));

            mockReminderSet.As<IQueryable<Models.Reminder>>()
                .Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Models.Reminder>(reminders.Provider));

            mockReminderSet.As<IQueryable<Models.Reminder>>().Setup(m => m.Provider).Returns(reminders.Provider);
            mockReminderSet.As<IQueryable<Models.Reminder>>().Setup(m => m.Expression).Returns(reminders.Expression);
            mockReminderSet.As<IQueryable<Models.Reminder>>().Setup(m => m.GetEnumerator()).Returns(reminders.GetEnumerator());

            mockReminderSet.Setup(m => m.Include("Schedules")).Returns(reminders.As<DbQuery<Reminder>>());

            mockScheduleSet.As<IDbAsyncEnumerable<Models.ReminderSchedule>>()
                .Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Models.ReminderSchedule>(reminderSchedules.GetEnumerator()));

            mockReminderSet.As<IQueryable<Models.ReminderSchedule>>()
                .Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Models.ReminderSchedule>(reminderSchedules.Provider));

            mockScheduleSet.As<IQueryable<Models.ReminderSchedule>>().Setup(m => m.Provider).Returns(reminderSchedules.Provider);
            mockScheduleSet.As<IQueryable<Models.ReminderSchedule>>().Setup(m => m.Expression).Returns(reminderSchedules.Expression);
            mockScheduleSet.As<IQueryable<Models.ReminderSchedule>>().Setup(m => m.GetEnumerator()).Returns(reminderSchedules.GetEnumerator());


            mockDbContext.Setup(db => db.Reminders).Returns(mockReminderSet.Object);
            mockDbContext.Setup(db => db.ReminderSchedules).Returns(mockScheduleSet.Object);

            // Act

            var controller = new Stuuck.Controllers.RemindersController(mockDbContext.Object);
            var actual = controller.GetReminders().ToList();

            // Assert

            actual.Should().NotBeNull();
            actual.First().Id.Should().Be(reminders.First().Id);
        }
    }
}
