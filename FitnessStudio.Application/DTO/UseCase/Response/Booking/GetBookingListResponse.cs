namespace FitnessStudio.Application.DTO.UseCase.Response.Booking
{
    public class GetBookingListResponse
    {
        public PaginatedList<BookingResponse> PaginatedList { get; }

        public GetBookingListResponse(PaginatedList<BookingResponse> paginatedList)
        {
            PaginatedList = paginatedList;
        }
    }
}
