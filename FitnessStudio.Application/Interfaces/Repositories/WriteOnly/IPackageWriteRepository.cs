using FitnessStudio.Domain.Entities.Package;

namespace FitnessStudio.Application.Interfaces.Repositories.WriteOnly
{
    public interface IPackageWriteRepository
    {
        Task<Package> PurchasePackageAsync(Guid userId, Guid businessStudioId, decimal totalCredits, DateTime expiryDate);
    }
}
