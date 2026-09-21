using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Application.DTO.UseCase.Response.Booking;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;

namespace FitnessStudio.Application.UseCase.GetBookingList
{
    public class GetBookingListUseCase : IGetBookingListUseCase
    {
        private readonly IBookingReadOnlyRepository _bookingReadOnlyRepository;

        public GetBookingListUseCase(IBookingReadOnlyRepository bookingReadOnlyRepository)
        {
            _bookingReadOnlyRepository = bookingReadOnlyRepository;
        }

        public async Task<GetBookingListResponse> Execute(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize)
        {
            var bookings = await _bookingReadOnlyRepository.GetBookingListAsync(userId, businessStudioId, pageNumber, pageSize);
            IList<BookingResponse> responses = bookings.Items.Select(x => new BookingResponse(x)).ToList();
            PaginatedList<BookingResponse> paginatedList = new PaginatedList<BookingResponse>(responses, bookings.TotalCount, bookings.PageNumber, pageSize);
            return new GetBookingListResponse(paginatedList);
        }
    }
}
