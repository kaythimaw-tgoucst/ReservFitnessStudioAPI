
namespace FitnessStudio.Application.DTO.UseCase.Response.Timetable
{
    public class GetTimetableResponse
    {
        public PaginatedList<TimetableResponse> PaginatedList { get; }

        public GetTimetableResponse(PaginatedList<TimetableResponse> paginatedList)
        {
            PaginatedList = paginatedList;
        }
    }
}
