using FitnessStudio.Domain.Entities.Booking;

namespace FitnessStudio.Application.DTO.UseCase.Response.Booking
{
    public class BookingResponse
    {
        public Guid Id { get; private set; }
        public string BookingNo { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public Guid TimetableScheduleId { get; private set; }
        public Guid PackageId { get; private set; }
        public string Status { get; private set; } = null!;
        public DateTime BookedOn { get; private set; }
        public DateTime? CancelledOn { get; private set; }
        public bool Waitlisted { get; private set; }

        public BookingResponse(FitnessStudio.Domain.Entities.Booking.Booking booking, bool waitlisted = false)
        {
            Id = booking.Id;
            BookingNo = booking.BookingNo;
            UserId = booking.UserId;
            TimetableScheduleId = booking.TimetableScheduleId;
            PackageId = booking.PackageId;
            Status = booking.Status;
            BookedOn = booking.BookedOn;
            CancelledOn = booking.CancelledOn;
            Waitlisted = waitlisted;
        }
    }
}
