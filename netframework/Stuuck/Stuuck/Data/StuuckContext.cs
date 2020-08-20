using Microsoft.Extensions.Configuration;
using System.Data.Entity;

namespace Stuuck.Data
{
    public class StuuckContext : DbContext
    {
        // ISSUE: can't do this, depends on Web.config
        //public StuuckContext() : base("name=StuuckContext") { }

        // TODO: replace Web.config with IConfigurationRoot
        public StuuckContext() : base(WebApiApplication.Configuration.GetConnectionString("StuuckContext"))
        {
        }

        public virtual System.Data.Entity.DbSet<Stuuck.Models.Reminder> Reminders { get; set; }
        public virtual System.Data.Entity.DbSet<Stuuck.Models.ReminderSchedule> ReminderSchedules { get; set; }
    }
}
