using FitnessStudio.Application.DTO.UseCase.Response.BusinessStudio;

namespace FitnessStudio.Application.UseCase.GetBusinessStudioList
{
    public interface IGetBusinessStudioListUseCase
    {
        Task<GetBusinessStudioListResponse> Execute(Guid userId);
    }
}
