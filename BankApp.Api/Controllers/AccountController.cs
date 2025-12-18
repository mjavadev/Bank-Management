using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserRequest request)
        {
            var result = await _userRepository.Authenticate(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequest request, [FromQuery] string role = "Customer")
        {
            var result = await _userRepository.Register(request, role);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("test-claims")]
        public IActionResult TestClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new { claims });
        }


        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found" });
            }

            var result = await _userRepository.ChangePassword(userId, request.CurrentPassword, request.NewPassword);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Account Controller is working!", timestamp = DateTime.Now });
        }
    }
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
