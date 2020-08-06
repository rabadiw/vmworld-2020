namespace Stuuck.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reminderschedule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReminderSchedules",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ReminderId = c.Long(nullable: false),
                        Schedule = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Reminders", t => t.ReminderId, cascadeDelete: true)
                .Index(t => t.ReminderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReminderSchedules", "ReminderId", "dbo.Reminders");
            DropIndex("dbo.ReminderSchedules", new[] { "ReminderId" });
            DropTable("dbo.ReminderSchedules");
        }
    }
}
