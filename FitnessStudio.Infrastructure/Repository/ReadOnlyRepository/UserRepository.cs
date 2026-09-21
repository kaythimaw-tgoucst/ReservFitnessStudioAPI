using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using FitnessStudio.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.Repository.ReadOnlyRepository
{
    public class UserRepository : IUserReadOnlyRepository
    {
        private readonly FitnessStudioDbContext _context;

        public UserRepository(FitnessStudioDbContext context)
        {
            _context = context;
        }

        public async Task<Guid?> GetUserIdByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim();

            return await _context.TUsers
                .AsNoTracking()
                .Where(x => x.Email == normalizedEmail)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync();
        }
    }
}
