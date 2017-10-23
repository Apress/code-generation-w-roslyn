using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class TimeEntriesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/TimeEntries
        public IQueryable<TimeEntry> GetTimeEntries()
        {
            return db.TimeEntries;
        }

        // GET: api/TimeEntries/5
        [ResponseType(typeof(TimeEntry))]
        public IHttpActionResult GetTimeEntry(int id)
        {
            TimeEntry timeEntry = db.TimeEntries.Find(id);
            if (timeEntry == null)
            {
                return NotFound();
            }

            return Ok(timeEntry);
        }

        // PUT: api/TimeEntries/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTimeEntry(int id, TimeEntry timeEntry)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != timeEntry.Id)
            {
                return BadRequest();
            }

            db.Entry(timeEntry).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeEntryExists(id))
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

        // POST: api/TimeEntries
        [ResponseType(typeof(TimeEntry))]
        public IHttpActionResult PostTimeEntry(TimeEntry timeEntry)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TimeEntries.Add(timeEntry);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = timeEntry.Id }, timeEntry);
        }

        // DELETE: api/TimeEntries/5
        [ResponseType(typeof(TimeEntry))]
        public IHttpActionResult DeleteTimeEntry(int id)
        {
            TimeEntry timeEntry = db.TimeEntries.Find(id);
            if (timeEntry == null)
            {
                return NotFound();
            }

            db.TimeEntries.Remove(timeEntry);
            db.SaveChanges();

            return Ok(timeEntry);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TimeEntryExists(int id)
        {
            return db.TimeEntries.Count(e => e.Id == id) > 0;
        }
    }
}