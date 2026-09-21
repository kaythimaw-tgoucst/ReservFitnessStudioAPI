using FitnessStudio.Application.DTO.UseCase.Request.Package;
using FitnessStudio.Application.DTO.UseCase.Response.Package;
using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;

namespace FitnessStudio.Application.UseCase.PurchasePackage
{
    public class PurchasePackageUseCase : IPurchasePackageUseCase
    {
        private readonly IPackageWriteRepository _packageWriteRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PurchasePackageUseCase(IPackageWriteRepository packageWriteRepository, IDateTimeProvider dateTimeProvider)
        {
            _packageWriteRepository = packageWriteRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<PurchasePackageResponse> Execute(Guid userId, PurchasePackageRequest request)
        {
            var package = await _packageWriteRepository.PurchasePackageAsync(
                userId,
                request.BusinessStudioId,
                request.TotalCredits,
                request.ExpiryDate);

            return new PurchasePackageResponse(new PackageResponse(package, _dateTimeProvider.GetCurrentDateUTC()));
        }
    }
}
