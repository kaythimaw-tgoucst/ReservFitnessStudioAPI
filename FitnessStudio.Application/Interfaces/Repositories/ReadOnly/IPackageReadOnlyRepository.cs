using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Domain.Entities.Package;

namespace FitnessStudio.Application.Interfaces.Repositories.ReadOnly
{
    public interface IPackageReadOnlyRepository
    {
        Task<PaginatedList<Package>> GetPackageListAsync(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize);
        Task<Package?> GetPackageByIdAsync(Guid packageId);
    }
}
