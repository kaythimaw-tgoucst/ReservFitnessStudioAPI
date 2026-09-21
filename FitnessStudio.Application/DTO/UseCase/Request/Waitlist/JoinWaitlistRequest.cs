using System.ComponentModel.DataAnnotations;

namespace FitnessStudio.Application.DTO.UseCase.Request.Waitlist
{
    public class JoinWaitlistRequest
    {
        [Required]
        public Guid TimetableScheduleId { get; set; }

        [Required]
        public Guid BusinessStudioId { get; set; }
    }
}
