using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using FitnessStudio.Application.UseCase.GetPackageList;
using FitnessStudio.Domain.Entities.Package;
using Moq;
using Xunit;

namespace FitnessStudio.Tests.UseCase.GetPackageList
{
    public class GetPackageListUseCaseTests
    {
        private readonly Mock<IPackageReadOnlyRepository> _packageReadOnlyRepositoryMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly GetPackageListUseCase _sut;

        public GetPackageListUseCaseTests()
        {
            _packageReadOnlyRepositoryMock = new Mock<IPackageReadOnlyRepository>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _sut = new GetPackageListUseCase(_packageReadOnlyRepositoryMock.Object, _dateTimeProviderMock.Object);
        }

        [Fact]
        public async Task Execute_WhenPackagesExist_ReturnsMappedResponsesWithExpiryFlag()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessStudioId = Guid.NewGuid();
            const int pageNumber = 1;
            const int pageSize = 10;
            var now = DateTime.UtcNow;

            var packages = new List<Package>
            {
                new Package
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BusinessStudioId = businessStudioId,
                    TotalCredits = 10,
                    RemainingCredits = 8,
                    ExpiryDate = now.AddDays(30),
                    CreatedOn = now
                },
                new Package
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BusinessStudioId = businessStudioId,
                    TotalCredits = 5,
                    RemainingCredits = 0,
                    ExpiryDate = now.AddDays(-5),
                    CreatedOn = now
                }
            };

            _packageReadOnlyRepositoryMock
                .Setup(r => r.GetPackageListAsync(userId, businessStudioId, pageNumber, pageSize))
                .ReturnsAsync(new PaginatedList<Package>(packages, packages.Count, pageNumber, pageSize));

            _dateTimeProviderMock.Setup(p => p.GetCurrentDateUTC()).Returns(now);

            // Act
            var response = await _sut.Execute(userId, businessStudioId, pageNumber, pageSize);

            // Assert
            Assert.Equal(2, response.PaginatedList.Items.Count);
            Assert.False(response.PaginatedList.Items[0].IsExpired);
            Assert.True(response.PaginatedList.Items[1].IsExpired);
        }

        [Fact]
        public async Task Execute_WhenNoPackages_ReturnsEmptyPaginatedList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessStudioId = Guid.NewGuid();
            const int pageNumber = 1;
            const int pageSize = 10;

            _packageReadOnlyRepositoryMock
                .Setup(r => r.GetPackageListAsync(userId, businessStudioId, pageNumber, pageSize))
                .ReturnsAsync(new PaginatedList<Package>(new List<Package>(), 0, pageNumber, pageSize));

            _dateTimeProviderMock.Setup(p => p.GetCurrentDateUTC()).Returns(DateTime.UtcNow);

            // Act
            var response = await _sut.Execute(userId, businessStudioId, pageNumber, pageSize);

            // Assert
            Assert.Empty(response.PaginatedList.Items);
            Assert.Equal(0, response.PaginatedList.TotalCount);
        }
    }
}
