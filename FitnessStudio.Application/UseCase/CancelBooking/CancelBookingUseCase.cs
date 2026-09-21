using FitnessStudio.Application.DTO.UseCase.Request.Booking;
using FitnessStudio.Application.DTO.UseCase.Response.Booking;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;

namespace FitnessStudio.Application.UseCase.CancelBooking
{
    public class CancelBookingUseCase : ICancelBookingUseCase
    {
        private readonly IBookingWriteRepository _bookingWriteRepository;

        public CancelBookingUseCase(IBookingWriteRepository bookingWriteRepository)
        {
            _bookingWriteRepository = bookingWriteRepository;
        }

        public async Task<CancelBookingResponse> Execute(Guid userId, CancelBookingRequest request)
        {
            var result = await _bookingWriteRepository.CancelBookingAsync(request.BookingId, userId);
            return new CancelBookingResponse(request.BookingId, result.CreditRefunded, result.WaitlistPromoted, result.PromotedUserId);
        }
    }
}
