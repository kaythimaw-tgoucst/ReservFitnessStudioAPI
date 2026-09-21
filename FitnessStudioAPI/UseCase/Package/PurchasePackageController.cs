using FitnessStudio.Application.DTO.UseCase.Request.Package;
using FitnessStudio.Application.Interfaces.Identity;
using FitnessStudio.Application.UseCase.PurchasePackage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudio.API.UseCase.Package
{
    [ApiController]
    [Authorize]
    [Route("api/packages")]
    public class PurchasePackageController : Controller
    {
        private readonly IPurchasePackageUseCase _purchasePackageUseCase;
        private readonly ICurrentUserService _currentUserService;

        public PurchasePackageController(IPurchasePackageUseCase purchasePackageUseCase, ICurrentUserService currentUserService)
        {
            _purchasePackageUseCase = purchasePackageUseCase;
            _currentUserService = currentUserService;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchasePackageRequest request)
        {
            var result = await _purchasePackageUseCase.Execute(_currentUserService.UserId, request);
            return Ok(result);
        }
    }
}
