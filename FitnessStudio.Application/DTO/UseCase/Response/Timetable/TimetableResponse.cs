
using FitnessStudio.Domain.Entities.Timetable;

namespace FitnessStudio.Application.DTO.UseCase.Response.Timetable
{
    public class TimetableResponse
    {
        public Guid Id { get; private set; }
        public Guid BusinessStudioId { get; private set; }
        public string BusinessName { get; private set; } = null!;
        public string ClassName { get; private set; } = null!;
        public string InstructorName { get; private set; } = null!;
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public int Capacity { get; private set; }
        public int AttendanceCount { get; private set; }
        public int AvailableSlots { get; private set; }        
        public DateTime CreatedOn { get; private set; }
        public DateTime? UpdatedOn { get; private set; }

        public TimetableResponse(FitnessStudio.Domain.Entities.Timetable.Timetable timetable)
        {
            Id = timetable.Id;
            BusinessStudioId = timetable.BusinessStudioId;
            BusinessName = timetable.BusinessName;
            ClassName = timetable.ClassName;
            InstructorName = timetable.InstructorName;
            StartTime = timetable.StartTime;
            EndTime = timetable.EndTime;
            Capacity = timetable.Capacity;
            AttendanceCount = timetable.AttendanceCount;
            AvailableSlots = timetable.Capacity - timetable.AttendanceCount;            
            CreatedOn = timetable.CreatedOn;
            UpdatedOn = timetable.UpdatedOn;
        }
    }
}
