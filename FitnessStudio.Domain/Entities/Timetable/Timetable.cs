using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessStudio.Domain.Entities.Timetable
{
    public class Timetable
    {
        public Guid Id { get; set; }
        public Guid BusinessStudioId { get; set; }
        public string BusinessName { get; set; } = null!;
        public string ClassName { get; set; } = null!;
        public string InstructorName { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Capacity { get; set; }
        public int AttendanceCount { get; set; }        
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
