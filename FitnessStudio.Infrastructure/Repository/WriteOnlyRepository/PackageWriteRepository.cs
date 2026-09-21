using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;
using FitnessStudio.Domain.Entities.Package;
using FitnessStudio.Infrastructure.DataAccess.Entities;
using FitnessStudio.Infrastructure.DataAccessWriteOnly;

namespace FitnessStudio.Infrastructure.Repository.WriteOnlyRepository
{
    public class PackageWriteRepository : IPackageWriteRepository
    {
        private readonly FitnessStudioWriteDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PackageWriteRepository(FitnessStudioWriteDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Package> PurchasePackageAsync(Guid userId, Guid businessStudioId, decimal totalCredits, DateTime expiryDate)
        {
            var entity = new TPackage
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BusinessStudioId = businessStudioId,
                TotalCredits = totalCredits,
                RemainingCredits = totalCredits,
                ExpiryDate = expiryDate,
                CreatedOn = _dateTimeProvider.GetCurrentDateUTC()
            };

            _context.TPackages.Add(entity);
            await _context.SaveChangesAsync();

            return new Package
            {
                Id = entity.Id,
                UserId = entity.UserId,
                BusinessStudioId = entity.BusinessStudioId,
                TotalCredits = entity.TotalCredits,
                RemainingCredits = entity.RemainingCredits,
                ExpiryDate = entity.ExpiryDate,
                CreatedOn = entity.CreatedOn,
                UpdatedOn = entity.UpdatedOn
            };
        }
    }
}
