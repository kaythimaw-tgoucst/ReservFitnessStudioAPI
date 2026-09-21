using FitnessStudio.Application.DTO.UseCase.Response.Package;

namespace FitnessStudio.Application.DTO.UseCase.Response.Package
{
    public class PurchasePackageResponse
    {
        public PackageResponse Package { get; }

        public PurchasePackageResponse(PackageResponse package)
        {
            Package = package;
        }
    }
}
