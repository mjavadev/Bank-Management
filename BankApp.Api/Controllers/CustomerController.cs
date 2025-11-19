using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankApp.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerController(ICustomerRepository customerRepository, IHttpContextAccessor httpContextAccessor)
        {
            _customerRepository = customerRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerRepository.GetAllCustomers();
            return Ok(result);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetCustomerById(int id)
        //{
        //    var result = await _customerRepository.GetCustomerById(id);
        //    return Ok(result);
        //}
        /// Get customer by ID with IDOR protection

        /// Customer: Can only view their own profile

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            if (IsInRole("Customer"))
            {
                var currentUserId = GetCurrentUserId();
                var currentCustomer = await _customerRepository.GetCustomerByUserId(currentUserId);

                if (currentCustomer.Errors.Count > 0)
                {
                    return BadRequest(new { success = false, errors = currentCustomer.Errors });
                }

                if (currentCustomer.Response.CustomerID != id)
                {
                    return Forbid();
                }
            }

            var result = await _customerRepository.GetCustomerById(id);

            if (result.Errors.Count > 0)
            {
                return BadRequest(new { success = false, errors = result.Errors });
            }
            return Ok(new { success = true, data = result.Response });
        }

        private string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
        }

        private bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        [HttpGet("by-user/{userId}")]
      //  [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetCustomerByUserId(string userId)
        {
            var result = await _customerRepository.GetCustomerByUserId(userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto customer)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _customerRepository.UpdateCustomer(id,customer, userId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _customerRepository.DeleteCustomer(id, userId);
            return Ok(result);
        }
    }

}

