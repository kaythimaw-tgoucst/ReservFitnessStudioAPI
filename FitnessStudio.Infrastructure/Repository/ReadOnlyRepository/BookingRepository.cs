using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using FitnessStudio.Domain.Entities.Booking;
using FitnessStudio.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.Repository.ReadOnlyRepository
{
    public class BookingRepository : IBookingReadOnlyRepository
    {
        private readonly FitnessStudioDbContext _context;

        public BookingRepository(FitnessStudioDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Booking>> GetBookingListAsync(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize)
        {
            var query = _context.TBookings
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.TimetableSchedule.BusinessStudioId == businessStudioId);

            query = query.OrderByDescending(x => x.BookedOn);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new Booking
                {
                    Id = x.Id,
                    BookingNo = x.BookingNo,
                    UserId = x.UserId,
                    TimetableScheduleId = x.TimetableScheduleId,
                    PackageId = x.PackageId,
                    Status = x.Status,
                    BookedOn = x.BookedOn,
                    CancelledOn = x.CancelledOn,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                })
                .ToListAsync();

            return new PaginatedList<Booking>(items, totalCount, pageNumber, pageSize);
        }
    }
}
