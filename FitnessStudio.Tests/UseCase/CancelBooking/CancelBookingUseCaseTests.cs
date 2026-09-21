using FitnessStudio.Application.DTO.UseCase.Request.Booking;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;
using FitnessStudio.Application.UseCase.CancelBooking;
using Moq;
using Xunit;

namespace FitnessStudio.Tests.UseCase.CancelBooking
{
    public class CancelBookingUseCaseTests
    {
        private readonly Mock<IBookingWriteRepository> _bookingWriteRepositoryMock;
        private readonly CancelBookingUseCase _sut;

        public CancelBookingUseCaseTests()
        {
            _bookingWriteRepositoryMock = new Mock<IBookingWriteRepository>();
            _sut = new CancelBookingUseCase(_bookingWriteRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_WhenCreditRefundedAndWaitlistPromoted_ReturnsExpectedResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new CancelBookingRequest
            {
                BookingId = Guid.NewGuid()
            };
            var promotedUserId = Guid.NewGuid();

            _bookingWriteRepositoryMock
                .Setup(r => r.CancelBookingAsync(request.BookingId, userId))
                .ReturnsAsync(new CancelBookingResult
                {
                    CreditRefunded = true,
                    WaitlistPromoted = true,
                    PromotedUserId = promotedUserId
                });

            // Act
            var response = await _sut.Execute(userId, request);

            // Assert
            Assert.Equal(request.BookingId, response.BookingId);
            Assert.True(response.CreditRefunded);
            Assert.True(response.WaitlistPromoted);
            Assert.Equal(promotedUserId, response.PromotedUserId);
        }

        [Fact]
        public async Task Execute_WhenNoCreditRefundedAndNoPromotion_ReturnsExpectedResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new CancelBookingRequest
            {
                BookingId = Guid.NewGuid()
            };

            _bookingWriteRepositoryMock
                .Setup(r => r.CancelBookingAsync(request.BookingId, userId))
                .ReturnsAsync(new CancelBookingResult
                {
                    CreditRefunded = false,
                    WaitlistPromoted = false,
                    PromotedUserId = null
                });

            // Act
            var response = await _sut.Execute(userId, request);

            // Assert
            Assert.Equal(request.BookingId, response.BookingId);
            Assert.False(response.CreditRefunded);
            Assert.False(response.WaitlistPromoted);
            Assert.Null(response.PromotedUserId);
        }
    }
}
