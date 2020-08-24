using System;
using System.Collections.Generic;

namespace Stuuck.Models
{
    public class Reminder
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual List<ReminderSchedule> Schedules { get; set; }
    }

    public class ReminderSchedule
    {
        public long Id { get; set; }
        public long ReminderId { get; set; }
        public virtual Reminder Reminder { get; set; }
        public DateTime Schedule { get; set; }
    }

    public class ReminderDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool HasSchedule { get; set; }
        public ICollection<ScheduleDto> Schedule { get; set; }
    }

    public class ScheduleDto
    {
        public long Id { get; set; }
        public DateTime Schedule { get; set; }
    }
}