using FitnessStudio.Application.DTO.UseCase.Request.Waitlist;
using FitnessStudio.Application.DTO.UseCase.Response.Waitlist;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;

namespace FitnessStudio.Application.UseCase.JoinWaitlist
{
    public class JoinWaitlistUseCase : IJoinWaitlistUseCase
    {
        private readonly IWaitlistWriteRepository _waitlistWriteRepository;

        public JoinWaitlistUseCase(IWaitlistWriteRepository waitlistWriteRepository)
        {
            _waitlistWriteRepository = waitlistWriteRepository;
        }

        public async Task<WaitlistResponse> Execute(Guid userId, JoinWaitlistRequest request)
        {
            var waitlist = await _waitlistWriteRepository.JoinWaitlistAsync(userId, request.TimetableScheduleId, request.BusinessStudioId);
            return new WaitlistResponse(waitlist);
        }
    }
}
