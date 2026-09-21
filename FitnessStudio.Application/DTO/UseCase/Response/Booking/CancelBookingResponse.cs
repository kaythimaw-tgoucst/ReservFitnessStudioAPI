namespace FitnessStudio.Application.DTO.UseCase.Response.Booking
{
    public class CancelBookingResponse
    {
        public Guid BookingId { get; }
        public bool CreditRefunded { get; }
        public bool WaitlistPromoted { get; }
        public Guid? PromotedUserId { get; }

        public CancelBookingResponse(Guid bookingId, bool creditRefunded, bool waitlistPromoted, Guid? promotedUserId)
        {
            BookingId = bookingId;
            CreditRefunded = creditRefunded;
            WaitlistPromoted = waitlistPromoted;
            PromotedUserId = promotedUserId;
        }
    }
}
