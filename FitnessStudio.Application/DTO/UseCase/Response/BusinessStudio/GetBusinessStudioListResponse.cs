
namespace FitnessStudio.Application.DTO.UseCase.Response.BusinessStudio
{
    public class GetBusinessStudioListResponse
    {
        public IList<BusinessStudioResponse> BusinessStudios { get; }

        public GetBusinessStudioListResponse(IList<BusinessStudioResponse> businessStudios)
        {
            BusinessStudios = businessStudios;
        }
    }
}
