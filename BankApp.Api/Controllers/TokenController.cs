using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;

        public TokenController(IUserRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost("GetToken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetToken([FromBody] UserRequest request)
        {
            Result<string> response = new();

            try
            {
                string username = _configuration["Jwt:ClientId"];
                string password = _configuration["Jwt:ClientSecret"];
                string key = _configuration["Jwt:Key"];

                if (!(request.UserName.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                      request.Password == password))
                {
                    response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = "Unauthorized" });
                    return BadRequest(response);
                }

                var symmKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var signInKey = new SigningCredentials(symmKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: signInKey);

                response.Response = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "500", ErrorMessage = ex.Message });
                return BadRequest(response);
            }
        }

        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<UserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Authenticate([FromBody] UserRequest request)
        {
            Result<UserResponse> response = new();

            try
            {
                var authResult = await _repository.Authenticate(request);

                if (authResult.IsError)
                {
                    return BadRequest(authResult);
                }

                var user = authResult.Response;
                string key = _configuration["Jwt:Key"];

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim("UserId", user.Id),
                    new Claim("FullName", user.FullName)
                };

                if (user.Roles != null && user.Roles.Any())
                {
                    foreach (var role in user.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                var symmKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var signInKey = new SigningCredentials(symmKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(8),
                    signingCredentials: signInKey);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                user.Token = tokenString;
                response.Response = user;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "500", ErrorMessage = ex.Message });
                return BadRequest(response);
            }
        }
    }
}
