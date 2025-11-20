using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountRepository _accountRepository;

        public BankAccountController(IBankAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // Get all accounts for a customer
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetAccountsByCustomerId(int customerId)
        {
            var result = await _accountRepository.GetAccountsByCustomerId(customerId);
            return Ok(result);
        }

        // Get account by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var result = await _accountRepository.GetAccountById(id);
            return Ok(result);
        }

        // Create new account for existing customer
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountDto accountDto)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _accountRepository.CreateAccount(accountDto, userId);
            return Ok(result);
        }

        // Deactivate account (soft delete)
        [HttpDelete("deactivate/{id}")]
        public async Task<IActionResult> DeactivateAccount(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _accountRepository.DeactivateAccount(id, userId);
            return Ok(result);
        }

        // Toggle account status (Active/Inactive)
        [HttpPut("toggle-status/{id}")]
        public async Task<IActionResult> ToggleAccountStatus(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _accountRepository.ToggleAccountStatus(id, userId);
            return Ok(result);
        }

    }

}

