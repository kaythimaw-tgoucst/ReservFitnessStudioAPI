using FitnessStudio.Domain.Entities.Waitlist;

namespace FitnessStudio.Application.Interfaces.Repositories.WriteOnly
{
    public interface IWaitlistWriteRepository
    {
        Task<Waitlist> JoinWaitlistAsync(Guid userId, Guid timetableScheduleId, Guid businessStudioId);
    }
}
