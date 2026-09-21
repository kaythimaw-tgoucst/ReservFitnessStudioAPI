using FitnessStudio.Application.DTO.UseCase.Request.Waitlist;
using FitnessStudio.Application.Exceptions;
using FitnessStudio.Application.Interfaces.Identity;
using FitnessStudio.Application.UseCase.JoinWaitlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudio.API.UseCase.Waitlist
{
    [ApiController]
    [Authorize]
    [Route("api/waitlist")]
    public class JoinWaitlistController : Controller
    {
        private readonly IJoinWaitlistUseCase _joinWaitlistUseCase;
        private readonly ICurrentUserService _currentUserService;

        public JoinWaitlistController(IJoinWaitlistUseCase joinWaitlistUseCase, ICurrentUserService currentUserService)
        {
            _joinWaitlistUseCase = joinWaitlistUseCase;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<IActionResult> Join([FromBody] JoinWaitlistRequest request)
        {
            try
            {
                var result = await _joinWaitlistUseCase.Execute(_currentUserService.UserId, request);
                return Ok(result);
            }
            catch (BookingException ex)
            {
                return BadRequest(new { errorCode = ex.ErrorCode, message = ex.Message });
            }
        }
    }
}
