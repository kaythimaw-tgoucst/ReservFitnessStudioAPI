using FitnessStudio.Application.DTO.UseCase.Request.Package;
using FitnessStudio.Application.DTO.UseCase.Response.Package;

namespace FitnessStudio.Application.UseCase.PurchasePackage
{
    public interface IPurchasePackageUseCase
    {
        Task<PurchasePackageResponse> Execute(Guid userId, PurchasePackageRequest request);
    }
}
