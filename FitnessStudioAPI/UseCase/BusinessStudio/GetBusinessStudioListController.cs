using FitnessStudio.Application.Interfaces.Identity;
using FitnessStudio.Application.UseCase.GetBusinessStudioList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudio.API.UseCase.BusinessStudio
{
    [ApiController]
    [Authorize]
    [Route("api/businessstudio")]
    public class GetBusinessStudioListController : Controller
    {
        private readonly IGetBusinessStudioListUseCase _getBusinessStudioListUseCase;
        private readonly ICurrentUserService _currentUserService;

        public GetBusinessStudioListController(
            IGetBusinessStudioListUseCase getBusinessStudioListUseCase,
            ICurrentUserService currentUserService)
        {
            _getBusinessStudioListUseCase = getBusinessStudioListUseCase;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBusinessStudioList()
        {
            var result = await _getBusinessStudioListUseCase.Execute(_currentUserService.UserId);
            return Ok(result);
        }
    }
}
