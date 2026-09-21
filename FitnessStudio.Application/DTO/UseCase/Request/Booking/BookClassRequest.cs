using System.ComponentModel.DataAnnotations;

namespace FitnessStudio.Application.DTO.UseCase.Request.Booking
{
    public class BookClassRequest
    {
        [Required]
        public Guid TimetableScheduleId { get; set; }

        [Required]
        public Guid BusinessStudioId { get; set; }
    }
}
