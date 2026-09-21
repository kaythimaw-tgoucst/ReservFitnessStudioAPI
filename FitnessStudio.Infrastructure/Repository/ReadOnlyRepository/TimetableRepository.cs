using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using FitnessStudio.Domain.Entities.Timetable;
using FitnessStudio.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.Repository.ReadOnlyRepository
{
    public class TimetableRepository : ITimetableReadOnlyRepository
    {
        private readonly FitnessStudioDbContext _context;

        public TimetableRepository(FitnessStudioDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Timetable>> GetTimetableAsync(
            Guid businessStudioId,
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.TTimetableSchedules
                .AsNoTracking()
                .Where(x => x.BusinessStudioId == businessStudioId);

            if (startDate.HasValue)
            {
                query = query.Where(x => x.StartTime >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.EndTime <= endDate.Value);
            }

            query = query
                .OrderBy(x => x.BusinessStudio.Name)
                .ThenBy(x => x.StartTime);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new Timetable
                {
                    Id = x.Id,
                    BusinessStudioId = x.BusinessStudioId,
                    ClassName = x.ClassName,
                    InstructorName = x.InstructorName,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Capacity = x.Capacity,
                    AttendanceCount = x.TBookings.Count(b => b.Status == "Booked"),
                    BusinessName = x.BusinessStudio.Name,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                })
                .ToListAsync();

            return new PaginatedList<Timetable>(items, totalCount, pageNumber, pageSize);
        }
    }
}
