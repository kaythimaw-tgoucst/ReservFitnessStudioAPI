using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Application.DTO.UseCase.Response.Package;
using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;

namespace FitnessStudio.Application.UseCase.GetPackageList
{
    public class GetPackageListUseCase : IGetPackageListUseCase
    {
        private readonly IPackageReadOnlyRepository _packageReadOnlyRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetPackageListUseCase(IPackageReadOnlyRepository packageReadOnlyRepository, IDateTimeProvider dateTimeProvider)
        {
            _packageReadOnlyRepository = packageReadOnlyRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<GetPackageListResponse> Execute(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize)
        {
            var packages = await _packageReadOnlyRepository.GetPackageListAsync(userId, businessStudioId, pageNumber, pageSize);
            var currentDateUtc = _dateTimeProvider.GetCurrentDateUTC();
            IList<PackageResponse> responses = packages.Items.Select(x => new PackageResponse(x, currentDateUtc)).ToList();
            PaginatedList<PackageResponse> paginatedList = new PaginatedList<PackageResponse>(responses, packages.TotalCount, packages.PageNumber, pageSize);
            return new GetPackageListResponse(paginatedList);
        }
    }
}
