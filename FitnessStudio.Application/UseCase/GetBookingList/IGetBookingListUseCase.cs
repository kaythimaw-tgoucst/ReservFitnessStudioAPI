using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.DTO.UseCase.Response.Booking;

namespace FitnessStudio.Application.UseCase.GetBookingList
{
    public interface IGetBookingListUseCase
    {
        Task<GetBookingListResponse> Execute(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize);
    }
}
