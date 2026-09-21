using FitnessStudio.Application.DTO.UseCase.Request.Booking;
using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Identity;
using FitnessStudio.Application.UseCase.CancelBooking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudio.API.UseCase.Booking
{
    [ApiController]
    [Authorize]
    [Route("api/bookings")]
    public class CancelBookingController : Controller
    {
        private readonly ICancelBookingUseCase _cancelBookingUseCase;
        private readonly ICurrentUserService _currentUserService;

        public CancelBookingController(ICancelBookingUseCase cancelBookingUseCase, ICurrentUserService currentUserService)
        {
            _cancelBookingUseCase = cancelBookingUseCase;
            _currentUserService = currentUserService;
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelBookingRequest request)
        {
            try
            {
                var result = await _cancelBookingUseCase.Execute(_currentUserService.UserId, request);
                return Ok(result);
            }
            catch (BookingException ex)
            {
                return BadRequest(new { errorCode = ex.ErrorCode, message = ex.Message });
            }
        }
    }
}
