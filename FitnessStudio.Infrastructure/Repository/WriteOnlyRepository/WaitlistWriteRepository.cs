using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Concurrency;
using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;
using FitnessStudio.Domain.Entities.Waitlist;
using FitnessStudio.Infrastructure.DataAccess.Entities;
using FitnessStudio.Infrastructure.DataAccessWriteOnly;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.Repository.WriteOnlyRepository
{
    public class WaitlistWriteRepository : IWaitlistWriteRepository
    {
        private readonly FitnessStudioWriteDbContext _context;
        private readonly IBookingConcurrencyLock _concurrencyLock;
        private readonly IDateTimeProvider _dateTimeProvider;

        public WaitlistWriteRepository(FitnessStudioWriteDbContext context, IBookingConcurrencyLock concurrencyLock, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _concurrencyLock = concurrencyLock;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Waitlist> JoinWaitlistAsync(Guid userId, Guid timetableScheduleId, Guid businessStudioId)
        {
            await using var handle = await _concurrencyLock.AcquireAsync($"booking:schedule:{timetableScheduleId}");

            var schedule = await _context.TTimetableSchedules
                .FirstOrDefaultAsync(x => x.Id == timetableScheduleId);
            if (schedule is null)
            {
                throw new BookingException("SCHEDULE_NOT_FOUND", "Timetable schedule not found.");
            }

            if (schedule.BusinessStudioId != businessStudioId)
            {
                throw new BookingException("BUSINESS_MISMATCH", "Timetable schedule does not belong to the given business studio.");
            }

            if (schedule.EndTime <= _dateTimeProvider.GetCurrentDateUTC())
            {
                throw new BookingException("SCHEDULE_ENDED", "Cannot join waitlist for a timetable schedule that has already ended.");
            }

            var attendanceCount = await _context.TBookings
                .CountAsync(x => x.TimetableScheduleId == timetableScheduleId && x.Status == "Booked");
            if (attendanceCount < schedule.Capacity)
            {
                throw new BookingException("SCHEDULE_HAS_AVAILABLE_SLOT", "Timetable schedule has available slot. Please book directly.");
            }

            var alreadyBooked = await _context.TBookings
                .AnyAsync(x => x.UserId == userId && x.TimetableScheduleId == timetableScheduleId && x.Status == "Booked");
            if (alreadyBooked)
            {
                throw new BookingException("ALREADY_BOOKED", "User already has an active booking for this schedule.");
            }

            var alreadyWaiting = await _context.TBookingWaitlists
                .AnyAsync(x => x.UserId == userId && x.TimetableScheduleId == timetableScheduleId && x.Status == "Waiting");
            if (alreadyWaiting)
            {
                throw new BookingException("ALREADY_WAITLISTED", "User is already on the waitlist for this schedule.");
            }

            var entity = new TBookingWaitlist
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TimetableScheduleId = timetableScheduleId,
                Status = "Waiting",
                JoinedAt = _dateTimeProvider.GetCurrentDateUTC(),
                CreatedOn = _dateTimeProvider.GetCurrentDateUTC()
            };

            _context.TBookingWaitlists.Add(entity);
            await _context.SaveChangesAsync();

            return new Waitlist
            {
                Id = entity.Id,
                UserId = entity.UserId,
                TimetableScheduleId = entity.TimetableScheduleId,
                Status = entity.Status,
                JoinedAt = entity.JoinedAt,
                PromotedAt = entity.PromotedAt,
                CreatedOn = entity.CreatedOn
            };
        }
    }
}
