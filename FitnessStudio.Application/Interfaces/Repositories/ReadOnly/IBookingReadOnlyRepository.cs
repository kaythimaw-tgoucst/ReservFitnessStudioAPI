using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Domain.Entities.Booking;

namespace FitnessStudio.Application.Interfaces.Repositories.ReadOnly
{
    public interface IBookingReadOnlyRepository
    {
        Task<PaginatedList<Booking>> GetBookingListAsync(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize);
    }
}
