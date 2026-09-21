using FitnessStudio.Domain.Entities.BusinessStudio;

namespace FitnessStudio.Application.Interfaces.Repositories.ReadOnly
{
    public interface IBusinessStudioReadOnlyRepository
    {
        Task<IList<BusinessStudio>> GetBusinessStudioListAsync(Guid userId);

        Task<bool> UserBelongsToBusinessStudioAsync(Guid userId, Guid businessStudioId);
    }
}
