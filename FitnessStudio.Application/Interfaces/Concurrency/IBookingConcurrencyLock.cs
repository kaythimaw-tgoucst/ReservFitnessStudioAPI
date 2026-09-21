namespace FitnessStudio.Application.Interfaces.Concurrency
{
    /// <summary>
    /// Distributed lock abstraction backed by Redis, used to serialize the
    /// check-and-reserve critical section for a given timetable schedule
    /// across multiple API instances.
    /// </summary>
    public interface IBookingConcurrencyLock
    {
        /// <summary>
        /// Acquires a lock scoped to the given key. Returns an IAsyncDisposable that
        /// releases the lock when disposed. Throws BookingException if the lock
        /// could not be acquired within the timeout.
        /// </summary>
        Task<IAsyncDisposable> AcquireAsync(string key, TimeSpan? timeout = null);
    }
}
