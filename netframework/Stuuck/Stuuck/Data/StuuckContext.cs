﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Stuuck.Data
{
    public class StuuckContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public StuuckContext() : base("name=StuuckContext")
        {
        }

        public System.Data.Entity.DbSet<Stuuck.Models.Reminder> Reminders { get; set; }
        public System.Data.Entity.DbSet<Stuuck.Models.ReminderSchedule> ReminderSchedules { get; set; }
    }
}
