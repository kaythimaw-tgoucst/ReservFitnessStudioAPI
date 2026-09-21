using FitnessStudio.Domain.Entities.Booking;

namespace FitnessStudio.Application.Interfaces.Repositories.WriteOnly
{
    public class BookClassResult
    {
        public Booking? Booking { get; set; }
        public bool Waitlisted { get; set; }
    }

    public class CancelBookingResult
    {
        public bool CreditRefunded { get; set; }
        public bool WaitlistPromoted { get; set; }
        public Guid? PromotedUserId { get; set; }
    }

    public interface IBookingWriteRepository
    {
        /// <summary>
        /// Attempts to book the given schedule for the user, using an eligible package
        /// (active, not expired, with remaining credits) owned by the user under the given business studio.
        /// Throws <see cref="Exceptions.BookingException"/> for business rule violations.
        /// If the schedule is full, returns Waitlisted = true and Booking = null so caller can use waitlist API.
        /// </summary>
        Task<BookClassResult> BookClassAsync(Guid userId, Guid timetableScheduleId, Guid businessStudioId);

        /// <summary>
        /// Cancels a booking, refunds a credit if cancelled more than 4 hours before class start,
        /// and promotes the first waitlisted user (if any) into the freed slot.
        /// </summary>
        Task<CancelBookingResult> CancelBookingAsync(Guid bookingId, Guid userId);
    }
}
