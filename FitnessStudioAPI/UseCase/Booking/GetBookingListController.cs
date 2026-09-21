using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.Interfaces.Identity;
using FitnessStudio.Application.UseCase.GetBookingList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudio.API.UseCase.Booking
{
    [ApiController]
    [Authorize]
    [Route("api/bookings")]
    public class GetBookingListController : Controller
    {
        private readonly IGetBookingListUseCase _getBookingListUseCase;
        private readonly ICurrentUserService _currentUserService;

        public GetBookingListController(IGetBookingListUseCase getBookingListUseCase, ICurrentUserService currentUserService)
        {
            _getBookingListUseCase = getBookingListUseCase;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings(Guid businessStudioId, int pageNumber = PaginationDefaults.PageNumber, int pageSize = PaginationDefaults.PageSize)
        {
            if (businessStudioId == Guid.Empty)
            {
                return BadRequest(new { message = "Business studio is required. Please provide a valid businessStudioId." });
            }

            var result = await _getBookingListUseCase.Execute(_currentUserService.UserId, businessStudioId, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
