using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{

    namespace WebApp.BankingApi.Controllers
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

            [HttpPost("authenticate")]
            public async Task<IActionResult> Authenticate([FromBody] UserRequest request)
            {
                var result = await _userRepository.Authenticate(request);
                return Ok(result);
            }

            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] UserRequest request, [FromQuery] string role = "Customer")
            {
                var result = await _userRepository.Register(request, role);
                return Ok(result);
            }

            [Authorize]
            [HttpPost("change-password")]
            public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _userRepository.ChangePassword(userId, request.CurrentPassword, request.NewPassword);
                return Ok(result);
            }
        }

        public class ChangePasswordRequest
        {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
        }
    }


}