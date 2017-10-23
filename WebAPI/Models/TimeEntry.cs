using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class TimeEntry
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ActivityId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndType { get; set; }
    }
}