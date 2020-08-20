using Stuuck.Data;
using Stuuck.Models;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Stuuck.Controllers
{
    public class RemindersController : ApiController
    {
        private readonly StuuckContext db = null;

        public RemindersController() : this(new StuuckContext()) { }

        // TODO: enable testability 
        public RemindersController(StuuckContext db)
        {
            this.db = db;
        }

        // GET: api/Reminders
        public IQueryable<ReminderDto> GetReminders()
        {
            var reminders = from r in db.Reminders
                            select new ReminderDto()
                            {
                                Id = r.Id,
                                Title = r.Title,
                                Description = r.Description,
                                HasSchedule = r.Schedules.Count > 0,
                                Schedule = r.Schedules.Select(s => new ScheduleDto()
                                {
                                    Id = s.Id,
                                    Schedule = s.Schedule
                                }).ToList()
                            };

            return reminders;
        }

        // GET: api/Reminders/5
        [ResponseType(typeof(ReminderDto))]
        public async Task<IHttpActionResult> GetReminder(long id)
        {
            //Reminder reminder = await db.Reminders.FindAsync(id);

            var reminder = await (from r in db.Reminders.Include(r => r.Schedules)
                                  where r.Id.Equals(id)
                                  select new ReminderDto()
                                  {
                                      Id = r.Id,
                                      Title = r.Title,
                                      Description = r.Description,
                                      HasSchedule = r.Schedules.Count > 0,
                                      Schedule = r.Schedules.Select(s => new ScheduleDto()
                                      {
                                          Id = s.Id,
                                          Schedule = s.Schedule
                                      }).ToList()
                                  }).SingleOrDefaultAsync();

            if (reminder == null)
            {
                return NotFound();
            }

            return Ok(reminder);
        }

        // PUT: api/Reminders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReminder(long id, ReminderDto reminderdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reminderdto.Id)
            {
                return BadRequest();
            }

            var reminder = await db.Reminders.FindAsync(id);
            db.Entry(reminder).State = EntityState.Modified;
            reminder.Title = reminderdto.Title;
            reminder.Description = reminderdto.Description;
            reminder.Schedules.AddRange(
                reminderdto.Schedule.Select(s => new ReminderSchedule()
                {
                    Id = s.Id,
                    ReminderId = id,
                    Schedule = s.Schedule
                }).ToList());

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReminderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Reminders
        [ResponseType(typeof(ReminderDto))]
        public async Task<IHttpActionResult> PostReminder(ReminderDto reminder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Reminders.Add(new Reminder()
            {
                Title = reminder.Title,
                Description = reminder.Description,
                Schedules = reminder?.Schedule?.Select(schedDto => new ReminderSchedule()
                {
                    Schedule = schedDto.Schedule
                }).ToList()
            });
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = reminder.Id }, reminder);
        }

        // DELETE: api/Reminders/5
        [ResponseType(typeof(Reminder))]
        public async Task<IHttpActionResult> DeleteReminder(long id)
        {
            Reminder reminder = await db.Reminders.FindAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }

            db.Reminders.Remove(reminder);
            await db.SaveChangesAsync();

            return Ok(reminder);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReminderExists(long id)
        {
            return db.Reminders.Count(e => e.Id == id) > 0;
        }
    }
}