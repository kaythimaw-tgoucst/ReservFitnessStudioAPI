using FitnessStudio.Domain.Entities.BusinessStudio;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using FitnessStudio.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.Repository.ReadOnlyRepository
{
    public class BusinessStudioRepository : IBusinessStudioReadOnlyRepository
    {
        private readonly FitnessStudioDbContext _context;

        public BusinessStudioRepository(FitnessStudioDbContext context)
        {
            _context = context;
        }

        public async Task<IList<BusinessStudio>> GetBusinessStudioListAsync(Guid userId)
        {
            var query = _context.TBusinessStudios
                .AsNoTracking()
                .Where(x => _context.TUserBusinessStudioMemberships
                    .Any(m => m.UserId == userId && m.BusinessStudioId == x.Id && m.IsActive));

            var items = await query
                .OrderBy(x => x.CompanyId)
                .ThenBy(x => x.Name)
                .Select(x => new BusinessStudio
                {
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Name,
                    Name = x.Name,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                })
                .ToListAsync();

            return items;
        }

        public async Task<bool> UserBelongsToBusinessStudioAsync(Guid userId, Guid businessStudioId)
        {
            return await _context.TUserBusinessStudioMemberships
                .AsNoTracking()
                .AnyAsync(m => m.UserId == userId && m.BusinessStudioId == businessStudioId && m.IsActive);
        }
    }
}
