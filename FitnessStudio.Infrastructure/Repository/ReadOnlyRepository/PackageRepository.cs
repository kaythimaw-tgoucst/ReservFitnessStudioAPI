using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using FitnessStudio.Domain.Entities.Package;
using FitnessStudio.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.Repository.ReadOnlyRepository
{
    public class PackageRepository : IPackageReadOnlyRepository
    {
        private readonly FitnessStudioDbContext _context;

        public PackageRepository(FitnessStudioDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Package>> GetPackageListAsync(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize)
        {
            var query = _context.TPackages
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.BusinessStudioId == businessStudioId);

            query = query.OrderByDescending(x => x.CreatedOn);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new Package
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    BusinessStudioId = x.BusinessStudioId,
                    TotalCredits = x.TotalCredits,
                    RemainingCredits = x.RemainingCredits,
                    ExpiryDate = x.ExpiryDate,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                })
                .ToListAsync();

            return new PaginatedList<Package>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<Package?> GetPackageByIdAsync(Guid packageId)
        {
            return await _context.TPackages
                .AsNoTracking()
                .Where(x => x.Id == packageId)
                .Select(x => new Package
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    BusinessStudioId = x.BusinessStudioId,
                    TotalCredits = x.TotalCredits,
                    RemainingCredits = x.RemainingCredits,
                    ExpiryDate = x.ExpiryDate,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                })
                .FirstOrDefaultAsync();
        }
    }
}
