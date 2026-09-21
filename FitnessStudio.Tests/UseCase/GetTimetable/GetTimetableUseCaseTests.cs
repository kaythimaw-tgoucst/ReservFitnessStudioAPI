using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using FitnessStudio.Application.UseCase.GetTimetable;
using FitnessStudio.Domain.Entities.Timetable;
using Moq;
using Xunit;

namespace FitnessStudio.Tests.UseCase.GetTimetable
{
    public class GetTimetableUseCaseTests
    {
        private readonly Mock<ITimetableReadOnlyRepository> _timetableReadOnlyRepositoryMock;
        private readonly Mock<IBusinessStudioReadOnlyRepository> _businessStudioReadOnlyRepositoryMock;
        private readonly GetTimetableUseCase _sut;

        public GetTimetableUseCaseTests()
        {
            _timetableReadOnlyRepositoryMock = new Mock<ITimetableReadOnlyRepository>();
            _businessStudioReadOnlyRepositoryMock = new Mock<IBusinessStudioReadOnlyRepository>();
            _sut = new GetTimetableUseCase(_timetableReadOnlyRepositoryMock.Object, _businessStudioReadOnlyRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_WhenUserBelongsToBusinessStudio_ReturnsMappedTimetable()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessStudioId = Guid.NewGuid();
            const int pageNumber = 1;
            const int pageSize = 10;

            _businessStudioReadOnlyRepositoryMock
                .Setup(r => r.UserBelongsToBusinessStudioAsync(userId, businessStudioId))
                .ReturnsAsync(true);

            var timetables = new List<Timetable>
            {
                new Timetable
                {
                    Id = Guid.NewGuid(),
                    BusinessStudioId = businessStudioId,
                    BusinessName = "Acme Fitness",
                    ClassName = "Yoga",
                    InstructorName = "Jane Doe",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddHours(1),
                    Capacity = 20,
                    AttendanceCount = 5,
                    CreatedOn = DateTime.UtcNow
                }
            };

            _timetableReadOnlyRepositoryMock
                .Setup(r => r.GetTimetableAsync(businessStudioId, pageNumber, pageSize, null, null))
                .ReturnsAsync(new PaginatedList<Timetable>(timetables, timetables.Count, pageNumber, pageSize));

            // Act
            var response = await _sut.Execute(userId, businessStudioId, pageNumber, pageSize);

            // Assert
            Assert.Single(response.PaginatedList.Items);
            Assert.Equal(15, response.PaginatedList.Items[0].AvailableSlots);
            Assert.Equal(timetables[0].Id, response.PaginatedList.Items[0].Id);
        }

        [Fact]
        public async Task Execute_WhenUserDoesNotBelongToBusinessStudio_ThrowsUserNotInBusinessStudioException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessStudioId = Guid.NewGuid();

            _businessStudioReadOnlyRepositoryMock
                .Setup(r => r.UserBelongsToBusinessStudioAsync(userId, businessStudioId))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotInBusinessStudioException>(
                () => _sut.Execute(userId, businessStudioId, 1, 10));

            _timetableReadOnlyRepositoryMock.Verify(
                r => r.GetTimetableAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()),
                Times.Never);
        }
    }
}
