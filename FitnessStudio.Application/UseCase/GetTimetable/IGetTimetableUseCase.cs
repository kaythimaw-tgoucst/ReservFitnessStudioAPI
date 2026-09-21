
using FitnessStudio.Application.DTO.UseCase.Response.Timetable;
using FitnessStudio.Application.Common.Constants;

namespace FitnessStudio.Application.UseCase.GetTimetable
{
    public interface IGetTimetableUseCase
    {
        Task<GetTimetableResponse> Execute(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null);
    }
}
