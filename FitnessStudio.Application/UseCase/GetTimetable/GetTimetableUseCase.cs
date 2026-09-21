using FitnessStudio.Application.DTO.UseCase.Response;
using FitnessStudio.Application.DTO.UseCase.Response.Timetable;
using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;

namespace FitnessStudio.Application.UseCase.GetTimetable
{
    public class GetTimetableUseCase : IGetTimetableUseCase
    {
        #region Member Variables
        private readonly ITimetableReadOnlyRepository _timetableReadOnlyRepository;
        private readonly IBusinessStudioReadOnlyRepository _businessStudioReadOnlyRepository;
        #endregion

        #region Constructor
        public GetTimetableUseCase(
            ITimetableReadOnlyRepository timetableReadOnlyRepository,
            IBusinessStudioReadOnlyRepository businessStudioReadOnlyRepository)
        {
            _timetableReadOnlyRepository = timetableReadOnlyRepository;
            _businessStudioReadOnlyRepository = businessStudioReadOnlyRepository;
        }
        #endregion

        #region Methods
        public async Task<GetTimetableResponse> Execute(
            Guid userId,
            Guid businessStudioId,
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            await ValidateUserBelongsToBusinessStudioAsync(userId, businessStudioId);

            var timetables = await _timetableReadOnlyRepository.GetTimetableAsync(businessStudioId, pageNumber, pageSize, startDate, endDate);

            IList<TimetableResponse> timetableResponses = timetables.Items.Select(x => new TimetableResponse(x)).ToList();
            PaginatedList<TimetableResponse> paginatedList = new PaginatedList<TimetableResponse>(timetableResponses, timetables.TotalCount, timetables.PageNumber, pageSize);
            GetTimetableResponse timetableResponse = new GetTimetableResponse(paginatedList);
            return timetableResponse;
        }

        private async Task ValidateUserBelongsToBusinessStudioAsync(Guid userId, Guid businessStudioId)
        {
            var userBelongsToBusinessStudio = await _businessStudioReadOnlyRepository.UserBelongsToBusinessStudioAsync(userId, businessStudioId);

            if (!userBelongsToBusinessStudio)
            {
                throw new UserNotInBusinessStudioException(userId, businessStudioId);
            }
        }
        #endregion
    }
}
