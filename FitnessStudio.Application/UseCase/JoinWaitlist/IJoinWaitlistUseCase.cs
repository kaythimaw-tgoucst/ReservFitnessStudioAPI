using FitnessStudio.Application.DTO.UseCase.Request.Waitlist;
using FitnessStudio.Application.DTO.UseCase.Response.Waitlist;

namespace FitnessStudio.Application.UseCase.JoinWaitlist
{
    public interface IJoinWaitlistUseCase
    {
        Task<WaitlistResponse> Execute(Guid userId, JoinWaitlistRequest request);
    }
}
