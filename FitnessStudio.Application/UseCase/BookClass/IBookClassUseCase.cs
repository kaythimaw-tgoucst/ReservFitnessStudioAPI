using FitnessStudio.Application.DTO.UseCase.Request.Booking;
using FitnessStudio.Application.DTO.UseCase.Response.Booking;

namespace FitnessStudio.Application.UseCase.BookClass
{
    public interface IBookClassUseCase
    {
        Task<BookingResponse> Execute(Guid userId, BookClassRequest request);
    }
}
