using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Concurrency;
using FitnessStudio.Application.Interfaces.Gateways;
using StackExchange.Redis;

namespace FitnessStudio.Infrastructure.Concurrency
{
    /// <summary>
    /// Redis-backed distributed lock (SET NX PX + token-guarded release) used to
    /// serialize the booking critical section across all API instances so that
    /// slot-availability checks and reservations cannot race each other.
    /// </summary>
    public class RedisBookingConcurrencyLock : IBookingConcurrencyLock
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDateTimeProvider _dateTimeProvider;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan LockTtl = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(100);

        private const string ReleaseScript = @"
if redis.call('get', KEYS[1]) == ARGV[1] then
    return redis.call('del', KEYS[1])
else
    return 0
end";

        public RedisBookingConcurrencyLock(IConnectionMultiplexer connectionMultiplexer, IDateTimeProvider dateTimeProvider)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IAsyncDisposable> AcquireAsync(string key, TimeSpan? timeout = null)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var lockKey = $"lock:{key}";
            var token = Guid.NewGuid().ToString("N");
            var deadline = _dateTimeProvider.GetCurrentDateUTC() + (timeout ?? DefaultTimeout);

            while (_dateTimeProvider.GetCurrentDateUTC() < deadline)
            {
                var acquired = await db.StringSetAsync(lockKey, token, LockTtl, When.NotExists);
                if (acquired)
                {
                    return new RedisLockHandle(db, lockKey, token);
                }

                await Task.Delay(RetryDelay);
            }

            throw new BookingException("BOOKING_IN_PROGRESS", "Another booking request for this schedule is currently being processed. Please try again.");
        }

        private sealed class RedisLockHandle : IAsyncDisposable
        {
            private readonly IDatabase _db;
            private readonly string _lockKey;
            private readonly string _token;

            public RedisLockHandle(IDatabase db, string lockKey, string token)
            {
                _db = db;
                _lockKey = lockKey;
                _token = token;
            }

            public async ValueTask DisposeAsync()
            {
                await _db.ScriptEvaluateAsync(ReleaseScript, new RedisKey[] { _lockKey }, new RedisValue[] { _token });
            }
        }
    }
}
