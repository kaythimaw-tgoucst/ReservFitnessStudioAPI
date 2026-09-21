using System;

namespace FitnessStudio.Domain.Entities.Booking
{
    public static class BookingStatus
    {
        public const string Booked = "Booked";
        public const string Cancelled = "Cancelled";
    }

    public class Booking
    {
        public Guid Id { get; set; }
        public string BookingNo { get; set; } = null!;
        public Guid UserId { get; set; }
        public Guid TimetableScheduleId { get; set; }
        public Guid PackageId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime BookedOn { get; set; }
        public DateTime? CancelledOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
