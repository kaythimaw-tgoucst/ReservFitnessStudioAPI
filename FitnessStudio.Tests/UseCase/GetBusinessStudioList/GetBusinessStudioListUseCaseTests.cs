using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using FitnessStudio.Application.UseCase.GetBusinessStudioList;
using FitnessStudio.Domain.Entities.BusinessStudio;
using Moq;
using Xunit;

namespace FitnessStudio.Tests.UseCase.GetBusinessStudioList
{
    public class GetBusinessStudioListUseCaseTests
    {
        private readonly Mock<IBusinessStudioReadOnlyRepository> _businessStudioReadOnlyRepositoryMock;
        private readonly GetBusinessStudioListUseCase _sut;

        public GetBusinessStudioListUseCaseTests()
        {
            _businessStudioReadOnlyRepositoryMock = new Mock<IBusinessStudioReadOnlyRepository>();
            _sut = new GetBusinessStudioListUseCase(_businessStudioReadOnlyRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_WhenBusinessStudiosExist_ReturnsMappedResponses()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessStudios = new List<BusinessStudio>
            {
                new BusinessStudio
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    CompanyName = "Acme Fitness",
                    Name = "Downtown Studio",
                    CreatedOn = DateTime.UtcNow
                },
                new BusinessStudio
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    CompanyName = "Acme Fitness",
                    Name = "Uptown Studio",
                    CreatedOn = DateTime.UtcNow
                }
            };

            _businessStudioReadOnlyRepositoryMock
                .Setup(r => r.GetBusinessStudioListAsync(userId))
                .ReturnsAsync(businessStudios);

            // Act
            var response = await _sut.Execute(userId);

            // Assert
            Assert.Equal(2, response.BusinessStudios.Count);
            Assert.Equal(businessStudios[0].Id, response.BusinessStudios[0].Id);
            Assert.Equal(businessStudios[1].Name, response.BusinessStudios[1].Name);
        }

        [Fact]
        public async Task Execute_WhenNoBusinessStudios_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _businessStudioReadOnlyRepositoryMock
                .Setup(r => r.GetBusinessStudioListAsync(userId))
                .ReturnsAsync(new List<BusinessStudio>());

            // Act
            var response = await _sut.Execute(userId);

            // Assert
            Assert.Empty(response.BusinessStudios);
        }
    }
}
