using System;

namespace FitnessStudio.Domain.Entities.Waitlist
{
    public static class WaitlistStatus
    {
        public const string Waiting = "Waiting";
        public const string Promoted = "Promoted";
    }

    public class Waitlist
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TimetableScheduleId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime JoinedAt { get; set; }
        public DateTime? PromotedAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
