using FitnessStudio.Application.UseCase.GetTimetable;
using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudio.API.UseCase.Timetable
{
    [ApiController]
    [Authorize]
    [Route("api/timetable")]
    public class GetTimetableController : Controller
    {
        private readonly IGetTimetableUseCase _getTimetableUseCase;
        private readonly ICurrentUserService _currentUserService;

        public GetTimetableController(IGetTimetableUseCase getTimetableUseCase, ICurrentUserService currentUserService)
        {
            _getTimetableUseCase = getTimetableUseCase;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTimetable(Guid businessStudioId, int pageNumber = PaginationDefaults.PageNumber, int pageSize = PaginationDefaults.PageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var result = await _getTimetableUseCase.Execute(_currentUserService.UserId, businessStudioId, pageNumber, pageSize, startDate, endDate);
                return Ok(result);
            }
            catch (UserNotInBusinessStudioException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
