using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Infrastructure.Concurrency;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace FitnessStudio.Tests.Concurrency
{
    public class RedisBookingConcurrencyLockTests
    {
        private readonly Mock<IConnectionMultiplexer> _connectionMultiplexerMock;
        private readonly Mock<IDatabase> _databaseMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly RedisBookingConcurrencyLock _sut;

        public RedisBookingConcurrencyLockTests()
        {
            _connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();

            _connectionMultiplexerMock
                .Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_databaseMock.Object);

            _sut = new RedisBookingConcurrencyLock(_connectionMultiplexerMock.Object, _dateTimeProviderMock.Object);
        }

        [Fact]
        public async Task AcquireAsync_WhenLockIsFree_ReturnsHandleAndSetsKey()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _dateTimeProviderMock.Setup(p => p.GetCurrentDateUTC()).Returns(now);

            _databaseMock
                .Setup(d => d.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    It.IsAny<TimeSpan?>(),
                    When.NotExists,
                    CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            var handle = await _sut.AcquireAsync("schedule:123");

            // Assert
            Assert.NotNull(handle);
            _databaseMock.Verify(d => d.StringSetAsync(
                It.Is<RedisKey>(k => k == "lock:schedule:123"),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                When.NotExists,
                CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task AcquireAsync_WhenLockCannotBeAcquiredBeforeTimeout_ThrowsBookingException()
        {
            // Arrange: first call establishes the deadline, second call is already past it
            // so the retry loop exits immediately without needing to actually wait.
            var start = DateTime.UtcNow;
            _dateTimeProviderMock
                .SetupSequence(p => p.GetCurrentDateUTC())
                .Returns(start)
                .Returns(start.AddSeconds(10));

            _databaseMock
                .Setup(d => d.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    It.IsAny<TimeSpan?>(),
                    When.NotExists,
                    CommandFlags.None))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BookingException>(() => _sut.AcquireAsync("schedule:123"));
            Assert.Equal("BOOKING_IN_PROGRESS", exception.ErrorCode);
        }

        [Fact]
        public async Task DisposeAsync_OnHandle_InvokesReleaseScript()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _dateTimeProviderMock.Setup(p => p.GetCurrentDateUTC()).Returns(now);

            _databaseMock
                .Setup(d => d.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    It.IsAny<TimeSpan?>(),
                    When.NotExists,
                    CommandFlags.None))
                .ReturnsAsync(true);

            _databaseMock
                .Setup(d => d.ScriptEvaluateAsync(
                    It.IsAny<string>(),
                    It.IsAny<RedisKey[]>(),
                    It.IsAny<RedisValue[]>(),
                    It.IsAny<CommandFlags>()))
                .ReturnsAsync(RedisResult.Create(1));

            var handle = await _sut.AcquireAsync("schedule:123");

            // Act
            await handle.DisposeAsync();

            // Assert
            _databaseMock.Verify(d => d.ScriptEvaluateAsync(
                It.IsAny<string>(),
                It.Is<RedisKey[]>(keys => keys.Length == 1 && keys[0] == "lock:schedule:123"),
                It.IsAny<RedisValue[]>(),
                It.IsAny<CommandFlags>()), Times.Once);
        }
    }
}
