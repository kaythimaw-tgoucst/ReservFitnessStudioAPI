using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Concurrency;
using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;
using FitnessStudio.Domain.Entities.Booking;
using FitnessStudio.Infrastructure.DataAccess.Entities;
using FitnessStudio.Infrastructure.DataAccessWriteOnly;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.Repository.WriteOnlyRepository
{
    public class BookingWriteRepository : IBookingWriteRepository
    {
        private readonly FitnessStudioWriteDbContext _context;
        private readonly IBookingConcurrencyLock _concurrencyLock;
        private readonly IDateTimeProvider _dateTimeProvider;

        // Cancellations made more than this many hours before class start are refunded.
        private static readonly TimeSpan RefundWindow = TimeSpan.FromHours(4);

        public BookingWriteRepository(FitnessStudioWriteDbContext context, IBookingConcurrencyLock concurrencyLock, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _concurrencyLock = concurrencyLock;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<BookClassResult> BookClassAsync(Guid userId, Guid timetableScheduleId, Guid businessStudioId)
        {
            // Serialize concurrent booking attempts for this schedule via a Redis
            // distributed lock, then re-validate everything against the database
            // (source of truth) inside a transaction before mutating state.
            await using var handle = await _concurrencyLock.AcquireAsync($"booking:schedule:{timetableScheduleId}");

            await using var transaction = await _context.Database.BeginTransactionAsync();

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

            var currentDate = _dateTimeProvider.GetCurrentDateUTC();

            if (schedule.EndTime <= currentDate)
            {
                throw new BookingException("SCHEDULE_EXPIRED", "Timetable schedule has already ended and cannot be booked.");
            }

            var package = await _context.TPackages
                .Where(x => x.UserId == userId
                    && x.BusinessStudioId == businessStudioId
                    && x.ExpiryDate >= currentDate
                    && x.RemainingCredits >= 1)
                .OrderBy(x => x.ExpiryDate)
                .FirstOrDefaultAsync();
            if (package is null)
            {
                throw new BookingException("PACKAGE_NOT_FOUND", "No eligible package with remaining credits was found for this business studio.");
            }

            var overlapping = await _context.TBookings
                .Where(x => x.UserId == userId && x.Status == "Booked")
                .Join(_context.TTimetableSchedules, b => b.TimetableScheduleId, s => s.Id, (b, s) => s)
                .AnyAsync(s => s.StartTime < schedule.EndTime && schedule.StartTime < s.EndTime);
            if (overlapping)
            {
                throw new BookingException("OVERLAPPING_BOOKING", "User already has an overlapping booking.");
            }

            var attendanceCount = await _context.TBookings
                .CountAsync(x => x.TimetableScheduleId == timetableScheduleId && x.Status == "Booked");

            if (attendanceCount >= schedule.Capacity)
            {
                // Schedule is full: caller should join the waitlist instead of booking.
                return new BookClassResult { Booking = null, Waitlisted = true };
            }

            var bookingEntity = new TBooking
            {
                Id = Guid.NewGuid(),
                BookingNo = GenerateBookingNo(),
                UserId = userId,
                TimetableScheduleId = timetableScheduleId,
                PackageId = package.Id,
                Status = "Booked",
                BookedOn = _dateTimeProvider.GetCurrentDateUTC(),
                CreatedOn = _dateTimeProvider.GetCurrentDateUTC()
            };

            package.RemainingCredits -= 1;
            package.UpdatedOn = _dateTimeProvider.GetCurrentDateUTC();

            _context.TBookings.Add(bookingEntity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new BookClassResult
            {
                Booking = new Booking
                {
                    Id = bookingEntity.Id,
                    BookingNo = bookingEntity.BookingNo,
                    UserId = bookingEntity.UserId,
                    TimetableScheduleId = bookingEntity.TimetableScheduleId,
                    PackageId = bookingEntity.PackageId,
                    Status = bookingEntity.Status,
                    BookedOn = bookingEntity.BookedOn,
                    CreatedOn = bookingEntity.CreatedOn
                },
                Waitlisted = false
            };
        }

        public async Task<CancelBookingResult> CancelBookingAsync(Guid bookingId, Guid userId)
        {
            var booking = await _context.TBookings
                .FirstOrDefaultAsync(x => x.Id == bookingId);
            if (booking is null)
            {
                throw new BookingException("BOOKING_NOT_FOUND", "Booking not found.");
            }

            if (booking.UserId != userId)
            {
                throw new BookingException("BOOKING_NOT_OWNED", "Booking does not belong to this user.");
            }

            if (booking.Status != "Booked")
            {
                throw new BookingException("BOOKING_NOT_ACTIVE", "Booking is not active and cannot be cancelled.");
            }

            await using var handle = await _concurrencyLock.AcquireAsync($"booking:schedule:{booking.TimetableScheduleId}");
            await using var transaction = await _context.Database.BeginTransactionAsync();

            booking = await _context.TBookings
                .FirstOrDefaultAsync(x => x.Id == bookingId);
            if (booking is null)
            {
                throw new BookingException("BOOKING_NOT_FOUND", "Booking not found.");
            }

            if (booking.UserId != userId)
            {
                throw new BookingException("BOOKING_NOT_OWNED", "Booking does not belong to this user.");
            }

            if (booking.Status != "Booked")
            {
                throw new BookingException("BOOKING_NOT_ACTIVE", "Booking is not active and cannot be cancelled.");
            }

            var schedule = await _context.TTimetableSchedules
                .FirstOrDefaultAsync(x => x.Id == booking.TimetableScheduleId);
            if (schedule is null)
            {
                throw new BookingException("SCHEDULE_NOT_FOUND", "Timetable schedule not found.");
            }

            var package = await _context.TPackages
                .FirstOrDefaultAsync(x => x.Id == booking.PackageId);

            booking.Status = "Cancelled";
            booking.CancelledOn = _dateTimeProvider.GetCurrentDateUTC();
            booking.UpdatedOn = _dateTimeProvider.GetCurrentDateUTC();

            var creditRefunded = false;
            if (schedule.StartTime - _dateTimeProvider.GetCurrentDateUTC() > RefundWindow && package is not null)
            {
                package.RemainingCredits += 1;
                package.UpdatedOn = _dateTimeProvider.GetCurrentDateUTC();
                creditRefunded = true;
            }

            var result = new CancelBookingResult { CreditRefunded = creditRefunded };

            if (schedule.EndTime <= _dateTimeProvider.GetCurrentDateUTC())
            {
                var staleWaitlist = await _context.TBookingWaitlists
                    .Where(x => x.TimetableScheduleId == schedule.Id && x.Status == "Waiting")
                    .ToListAsync();

                foreach (var waitlistItem in staleWaitlist)
                {
                    waitlistItem.Status = "Expired";
                    waitlistItem.UpdatedOn = _dateTimeProvider.GetCurrentDateUTC();
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return result;
            }

            // Promote first-in-line waitlisted user (FIFO) into the freed slot.
            var nextWaitlist = await _context.TBookingWaitlists
                .Where(x => x.TimetableScheduleId == schedule.Id && x.Status == "Waiting")
                .OrderBy(x => x.JoinedAt)
                .FirstOrDefaultAsync();

            if (nextWaitlist is not null)
            {
                var promotedPackage = await _context.TPackages
                    .Where(x => x.UserId == nextWaitlist.UserId
                        && x.BusinessStudioId == schedule.BusinessStudioId
                        && x.ExpiryDate >= _dateTimeProvider.GetCurrentDateUTC()
                        && x.RemainingCredits >= 1)
                    .OrderBy(x => x.ExpiryDate)
                    .FirstOrDefaultAsync();

                if (promotedPackage is not null)
                {
                    var hasOverlappingBooking = await _context.TBookings
                        .Where(x => x.UserId == nextWaitlist.UserId && x.Status == "Booked")
                        .Join(_context.TTimetableSchedules, b => b.TimetableScheduleId, s => s.Id, (b, s) => s)
                        .AnyAsync(s => s.StartTime < schedule.EndTime && schedule.StartTime < s.EndTime);

                    if (hasOverlappingBooking)
                    {
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return result;
                    }

                    promotedPackage.RemainingCredits -= 1;
                    promotedPackage.UpdatedOn = DateTime.UtcNow;

                    var promotedBooking = new TBooking
                    {
                        Id = Guid.NewGuid(),
                        BookingNo = GenerateBookingNo(),
                        UserId = nextWaitlist.UserId,
                        TimetableScheduleId = schedule.Id,
                        PackageId = promotedPackage.Id,
                        Status = "Booked",
                        BookedOn = DateTime.UtcNow,
                        CreatedOn = DateTime.UtcNow
                    };
                    _context.TBookings.Add(promotedBooking);

                    nextWaitlist.Status = "Promoted";
                    nextWaitlist.PromotedAt = DateTime.UtcNow;
                    nextWaitlist.UpdatedOn = DateTime.UtcNow;

                    result.WaitlistPromoted = true;
                    result.PromotedUserId = nextWaitlist.UserId;
                }
                // If the waitlisted user has no valid package/credits, they remain waiting
                // and no credit is deducted (nothing to release, per requirements).
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return result;
        }

        private string GenerateBookingNo()
        {
            return $"BK-{_dateTimeProvider.GetCurrentDateUTC():yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        }
    }
}
