using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

     //   [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerRepository.GetAllCustomers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result = await _customerRepository.GetCustomerById(id);
            return Ok(result);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetCustomerByUserId(string userId)
        {
            var result = await _customerRepository.GetCustomerByUserId(userId);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto customer)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _customerRepository.UpdateCustomer(customer, userId);
            return Ok(result);
        }

    //    [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _customerRepository.DeleteCustomer(id, userId);
            return Ok(result);
        }
    }

}

