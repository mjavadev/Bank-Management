using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IWebHostEnvironment _webHost;

        public ApplicationController(IApplicationRepository applicationRepository, IWebHostEnvironment webHost)
        {
            _applicationRepository = applicationRepository;
            _webHost = webHost;
        }

        [AllowAnonymous]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateApplication([FromForm] ApplicationDto application, IFormFile? imageFile)
        {
            try
            {
                if (imageFile != null)
                {
                    var webRootPath = _webHost.WebRootPath;

                    if (string.IsNullOrEmpty(webRootPath))
                    {
                        webRootPath = Path.Combine(_webHost.ContentRootPath, "wwwroot");
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine(webRootPath, "CustomerImages");

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    var fullPath = Path.Combine(filePath, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    application.CustomerImageURL = "CustomerImages/" + uniqueFileName;
                }

                var result = await _applicationRepository.CreateApplication(application);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        //[Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllApplications()
        {
            var result = await _applicationRepository.GetAllApplications();
            return Ok(result);
        }

      //  [Authorize(Roles = "Admin,Manager")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingApplications()
        {
            var result = await _applicationRepository.GetPendingApplications();
            return Ok(result);
        }

      //  [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationById(int id)
        {
            var result = await _applicationRepository.GetApplicationById(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplication(int id, [FromBody] ApplicationDto application)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _applicationRepository.UpdateApplication(id, application, userId);
            return Ok(result);
        }


        //   [Authorize(Roles = "Admin,Manager")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _applicationRepository.ApproveApplication(id, userId);
            return Ok(result);
        }

       // [Authorize(Roles = "Admin,Manager")]
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectApplication(int id, [FromBody] RejectRequest request)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _applicationRepository.RejectApplication(id, userId, request.Reason);
            return Ok(result);
        }
    }

    public class RejectRequest
    {
        public string Reason { get; set; }
    }
}