using FitnessStudio.Application.DTO.UseCase.Request.Package;
using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;
using FitnessStudio.Application.UseCase.PurchasePackage;
using FitnessStudio.Domain.Entities.Package;
using Moq;
using Xunit;

namespace FitnessStudio.Tests.UseCase.PurchasePackage
{
    public class PurchasePackageUseCaseTests
    {
        private readonly Mock<IPackageWriteRepository> _packageWriteRepositoryMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly PurchasePackageUseCase _sut;

        public PurchasePackageUseCaseTests()
        {
            _packageWriteRepositoryMock = new Mock<IPackageWriteRepository>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _sut = new PurchasePackageUseCase(_packageWriteRepositoryMock.Object, _dateTimeProviderMock.Object);
        }

        [Fact]
        public async Task Execute_WhenPackagePurchased_ReturnsPurchasePackageResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var request = new PurchasePackageRequest
            {
                BusinessStudioId = Guid.NewGuid(),
                TotalCredits = 10,
                ExpiryDate = now.AddMonths(1)
            };

            var package = new Package
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BusinessStudioId = request.BusinessStudioId,
                TotalCredits = request.TotalCredits,
                RemainingCredits = request.TotalCredits,
                ExpiryDate = request.ExpiryDate,
                CreatedOn = now
            };

            _packageWriteRepositoryMock
                .Setup(r => r.PurchasePackageAsync(userId, request.BusinessStudioId, request.TotalCredits, request.ExpiryDate))
                .ReturnsAsync(package);

            _dateTimeProviderMock.Setup(p => p.GetCurrentDateUTC()).Returns(now);

            // Act
            var response = await _sut.Execute(userId, request);

            // Assert
            Assert.Equal(package.Id, response.Package.Id);
            Assert.Equal(package.TotalCredits, response.Package.TotalCredits);
            Assert.Equal(package.RemainingCredits, response.Package.RemainingCredits);
            Assert.False(response.Package.IsExpired);
        }

        [Fact]
        public async Task Execute_WhenExpiryDateInPast_ReturnsIsExpiredTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var request = new PurchasePackageRequest
            {
                BusinessStudioId = Guid.NewGuid(),
                TotalCredits = 5,
                ExpiryDate = now.AddDays(-1)
            };

            var package = new Package
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BusinessStudioId = request.BusinessStudioId,
                TotalCredits = request.TotalCredits,
                RemainingCredits = request.TotalCredits,
                ExpiryDate = request.ExpiryDate,
                CreatedOn = now
            };

            _packageWriteRepositoryMock
                .Setup(r => r.PurchasePackageAsync(userId, request.BusinessStudioId, request.TotalCredits, request.ExpiryDate))
                .ReturnsAsync(package);

            _dateTimeProviderMock.Setup(p => p.GetCurrentDateUTC()).Returns(now);

            // Act
            var response = await _sut.Execute(userId, request);

            // Assert
            Assert.True(response.Package.IsExpired);
        }
    }
}
