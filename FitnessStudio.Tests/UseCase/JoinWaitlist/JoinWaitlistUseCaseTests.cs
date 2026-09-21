using FitnessStudio.Application.DTO.UseCase.Request.Waitlist;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;
using FitnessStudio.Application.UseCase.JoinWaitlist;
using FitnessStudio.Domain.Entities.Waitlist;
using Moq;
using Xunit;

namespace FitnessStudio.Tests.UseCase.JoinWaitlist
{
    public class JoinWaitlistUseCaseTests
    {
        private readonly Mock<IWaitlistWriteRepository> _waitlistWriteRepositoryMock;
        private readonly JoinWaitlistUseCase _sut;

        public JoinWaitlistUseCaseTests()
        {
            _waitlistWriteRepositoryMock = new Mock<IWaitlistWriteRepository>();
            _sut = new JoinWaitlistUseCase(_waitlistWriteRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_WhenUserJoinsWaitlist_ReturnsWaitlistResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new JoinWaitlistRequest
            {
                TimetableScheduleId = Guid.NewGuid(),
                BusinessStudioId = Guid.NewGuid()
            };

            var waitlist = new Waitlist
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TimetableScheduleId = request.TimetableScheduleId,
                Status = WaitlistStatus.Waiting,
                JoinedAt = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            _waitlistWriteRepositoryMock
                .Setup(r => r.JoinWaitlistAsync(userId, request.TimetableScheduleId, request.BusinessStudioId))
                .ReturnsAsync(waitlist);

            // Act
            var response = await _sut.Execute(userId, request);

            // Assert
            Assert.Equal(waitlist.Id, response.Id);
            Assert.Equal(userId, response.UserId);
            Assert.Equal(request.TimetableScheduleId, response.TimetableScheduleId);
            Assert.Equal(WaitlistStatus.Waiting, response.Status);
            Assert.Null(response.PromotedAt);
        }
    }
}
