using Microsoft.EntityFrameworkCore;

namespace Stuuck.Data
{
    public class StuuckContext : DbContext
    {
        public StuuckContext(DbContextOptions options) : base(options)
        {

        }

        public Microsoft.EntityFrameworkCore.DbSet<Stuuck.Models.Reminder> Reminders { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Stuuck.Models.ReminderSchedule> ReminderSchedules { get; set; }
    }
}
