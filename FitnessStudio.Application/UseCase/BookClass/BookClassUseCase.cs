using FitnessStudio.Application.DTO.UseCase.Request.Booking;
using FitnessStudio.Application.DTO.UseCase.Response.Booking;
using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;
using FitnessStudio.Domain.Entities.Booking;

namespace FitnessStudio.Application.UseCase.BookClass
{
    public class BookClassUseCase : IBookClassUseCase
    {
        private readonly IBookingWriteRepository _bookingWriteRepository;

        public BookClassUseCase(IBookingWriteRepository bookingWriteRepository)
        {
            _bookingWriteRepository = bookingWriteRepository;
        }

        public async Task<BookingResponse> Execute(Guid userId, BookClassRequest request)
        {
            var result = await _bookingWriteRepository.BookClassAsync(userId, request.TimetableScheduleId, request.BusinessStudioId);

            if (result.Waitlisted || result.Booking is null)
            {
                // Schedule is full: caller should call the waitlist endpoint to join.
                throw new BookingException("SCHEDULE_FULL", "Timetable schedule is full. Please join the waitlist instead.");
            }

            return new BookingResponse(result.Booking);
        }
    }
}
