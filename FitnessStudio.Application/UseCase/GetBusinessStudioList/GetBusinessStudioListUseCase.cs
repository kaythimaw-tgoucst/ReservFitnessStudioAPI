using FitnessStudio.Application.DTO.UseCase.Response.BusinessStudio;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;

namespace FitnessStudio.Application.UseCase.GetBusinessStudioList
{
    public class GetBusinessStudioListUseCase : IGetBusinessStudioListUseCase
    {
        #region Member Variables
        private readonly IBusinessStudioReadOnlyRepository _businessStudioReadOnlyRepository;
        #endregion

        #region Constructor
        public GetBusinessStudioListUseCase(IBusinessStudioReadOnlyRepository businessStudioReadOnlyRepository)
        {
            _businessStudioReadOnlyRepository = businessStudioReadOnlyRepository;
        }
        #endregion

        #region Methods
        public async Task<GetBusinessStudioListResponse> Execute(Guid userId)
        {
            var businessStudios = await _businessStudioReadOnlyRepository.GetBusinessStudioListAsync(userId);

            IList<BusinessStudioResponse> businessStudioResponses = businessStudios.Select(x => new BusinessStudioResponse(x)).ToList();

            return new GetBusinessStudioListResponse(businessStudioResponses);
        }
        #endregion
    }
}
