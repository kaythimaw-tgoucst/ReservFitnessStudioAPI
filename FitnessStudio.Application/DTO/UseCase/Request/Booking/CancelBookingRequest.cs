using System.ComponentModel.DataAnnotations;

namespace FitnessStudio.Application.DTO.UseCase.Request.Booking
{
    public class CancelBookingRequest
    {
        [Required]
        public Guid BookingId { get; set; }
    }
}
