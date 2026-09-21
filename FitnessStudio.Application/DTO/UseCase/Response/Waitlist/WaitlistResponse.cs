using FitnessStudio.Domain.Entities.Waitlist;

namespace FitnessStudio.Application.DTO.UseCase.Response.Waitlist
{
    public class WaitlistResponse
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid TimetableScheduleId { get; private set; }
        public string Status { get; private set; } = null!;
        public DateTime JoinedAt { get; private set; }
        public DateTime? PromotedAt { get; private set; }

        public WaitlistResponse(FitnessStudio.Domain.Entities.Waitlist.Waitlist waitlist)
        {
            Id = waitlist.Id;
            UserId = waitlist.UserId;
            TimetableScheduleId = waitlist.TimetableScheduleId;
            Status = waitlist.Status;
            JoinedAt = waitlist.JoinedAt;
            PromotedAt = waitlist.PromotedAt;
        }
    }
}
