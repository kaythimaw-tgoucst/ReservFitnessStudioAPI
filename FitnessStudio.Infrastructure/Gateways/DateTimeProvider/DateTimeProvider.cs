using FitnessStudio.Application.Interfaces.Gateways;

namespace FitnessStudio.Infrastructure.Gateways.DateTimeProvider
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateUTC() => DateTime.UtcNow;        
    }
}
