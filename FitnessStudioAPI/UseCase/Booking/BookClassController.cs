using FitnessStudio.Application.DTO.UseCase.Request.Booking;
using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Identity;
using FitnessStudio.Application.UseCase.BookClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudio.API.UseCase.Booking
{
    [ApiController]
    [Authorize]
    [Route("api/bookings")]
    public class BookClassController : Controller
    {
        private readonly IBookClassUseCase _bookClassUseCase;
        private readonly ICurrentUserService _currentUserService;

        public BookClassController(IBookClassUseCase bookClassUseCase, ICurrentUserService currentUserService)
        {
            _bookClassUseCase = bookClassUseCase;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<IActionResult> Book([FromBody] BookClassRequest request)
        {
            try
            {
                var result = await _bookClassUseCase.Execute(_currentUserService.UserId, request);
                return Ok(result);
            }
            catch (BookingException ex)
            {
                return BadRequest(new { errorCode = ex.ErrorCode, message = ex.Message });
            }
        }
    }
}
