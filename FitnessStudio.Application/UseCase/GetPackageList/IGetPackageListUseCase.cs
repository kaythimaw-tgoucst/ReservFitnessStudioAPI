using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.DTO.UseCase.Response.Package;

namespace FitnessStudio.Application.UseCase.GetPackageList
{
    public interface IGetPackageListUseCase
    {
        Task<GetPackageListResponse> Execute(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize);
    }
}
