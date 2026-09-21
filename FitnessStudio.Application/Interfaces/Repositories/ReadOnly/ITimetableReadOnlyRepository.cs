using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Domain.Entities.Timetable;

namespace FitnessStudio.Application.Interfaces.Repositories.ReadOnly
{
    public interface ITimetableReadOnlyRepository
    {
        Task<PaginatedList<Timetable>> GetTimetableAsync(
            Guid businessStudioId,
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null);
    }
}
