using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FitnessStudio.API.UseCase.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class GetLoginUserController : Controller
    {
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly IConfiguration _configuration;

        public GetLoginUserController(IUserReadOnlyRepository userReadOnlyRepository, IConfiguration configuration)
        {
            _userReadOnlyRepository = userReadOnlyRepository;
            _configuration = configuration;
        }

        [HttpGet("account")]
        public async Task<IActionResult> GetUserIdByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required." });
            }

            var userId = await _userReadOnlyRepository.GetUserIdByEmailAsync(email);

            if (!userId.HasValue)
            {
                return NotFound(new { message = "User not found." });
            }

            var token = GenerateJwtToken(userId.Value);

            return Ok(new { userId = userId.Value, email = email.Trim(), token });
        }

        private string GenerateJwtToken(Guid userId)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
