using FitnessStudio.Application.DTO.UseCase.Request.Booking;
using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;
using FitnessStudio.Application.UseCase.BookClass;
using FitnessStudio.Domain.Entities.Booking;
using Moq;
using Xunit;

namespace FitnessStudio.Tests.UseCase.BookClass
{
    public class BookClassUseCaseTests
    {
        private readonly Mock<IBookingWriteRepository> _bookingWriteRepositoryMock;
        private readonly BookClassUseCase _sut;

        public BookClassUseCaseTests()
        {
            _bookingWriteRepositoryMock = new Mock<IBookingWriteRepository>();
            _sut = new BookClassUseCase(_bookingWriteRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_WhenScheduleHasAvailability_ReturnsBookingResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new BookClassRequest
            {
                TimetableScheduleId = Guid.NewGuid(),
                BusinessStudioId = Guid.NewGuid()
            };

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                BookingNo = "BK-0001",
                UserId = userId,
                TimetableScheduleId = request.TimetableScheduleId,
                PackageId = Guid.NewGuid(),
                Status = BookingStatus.Booked,
                BookedOn = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            _bookingWriteRepositoryMock
                .Setup(r => r.BookClassAsync(userId, request.TimetableScheduleId, request.BusinessStudioId))
                .ReturnsAsync(new BookClassResult { Booking = booking, Waitlisted = false });

            // Act
            var response = await _sut.Execute(userId, request);

            // Assert
            Assert.False(response.Waitlisted);
            Assert.Equal(booking.Id, response.Id);
            Assert.Equal(booking.BookingNo, response.BookingNo);
            Assert.Equal(BookingStatus.Booked, response.Status);
        }

        [Fact]
        public async Task Execute_WhenScheduleIsFull_ThrowsBookingException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new BookClassRequest
            {
                TimetableScheduleId = Guid.NewGuid(),
                BusinessStudioId = Guid.NewGuid()
            };

            _bookingWriteRepositoryMock
                .Setup(r => r.BookClassAsync(userId, request.TimetableScheduleId, request.BusinessStudioId))
                .ReturnsAsync(new BookClassResult { Booking = null, Waitlisted = true });

            // Act
            var exception = await Assert.ThrowsAsync<BookingException>(() => _sut.Execute(userId, request));

            // Assert
            Assert.Equal("SCHEDULE_FULL", exception.ErrorCode);
        }
    }
}
