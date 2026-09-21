using FitnessStudio.Application.Common.Constants;
using FitnessStudio.Application.Interfaces.Identity;
using FitnessStudio.Application.UseCase.GetPackageList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudio.API.UseCase.Package
{
    [ApiController]
    [Authorize]
    [Route("api/packages")]
    public class GetPackageController : Controller
    {
        private readonly IGetPackageListUseCase _getPackageListUseCase;
        private readonly ICurrentUserService _currentUserService;

        public GetPackageController(IGetPackageListUseCase getPackageListUseCase, ICurrentUserService currentUserService)
        {
            _getPackageListUseCase = getPackageListUseCase;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPackages(Guid businessStudioId, int pageNumber = PaginationDefaults.PageNumber, int pageSize = PaginationDefaults.PageSize)
        {
            if (businessStudioId == Guid.Empty)
            {
                return BadRequest(new { message = "Business studio is required. Please provide a valid businessStudioId." });
            }

            var result = await _getPackageListUseCase.Execute(_currentUserService.UserId, businessStudioId, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
