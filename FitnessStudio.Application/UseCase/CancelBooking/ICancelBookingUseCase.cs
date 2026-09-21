using FitnessStudio.Application.DTO.UseCase.Request.Booking;
using FitnessStudio.Application.DTO.UseCase.Response.Booking;

namespace FitnessStudio.Application.UseCase.CancelBooking
{
    public interface ICancelBookingUseCase
    {
        Task<CancelBookingResponse> Execute(Guid userId, CancelBookingRequest request);
    }
}
