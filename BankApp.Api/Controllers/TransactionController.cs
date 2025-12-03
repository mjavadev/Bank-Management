using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.BankingApi.Entity.Models;

namespace BankApp.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ApplicationDbContext _context;

        public TransactionController(ITransactionRepository transactionRepository,
            ApplicationDbContext context)
        {
            _transactionRepository = transactionRepository;
            _context = context;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var result = await _transactionRepository.GetAllTransactions();
            return Ok(result);
        }

        [HttpGet("account/{accountId}")]

        public async Task<IActionResult> GetTransactionsByAccountId(int accountId)

        {

            var userId = User.FindFirstValue("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            var account = await _context.Accounts

                .Include(a => a.Customer)

                .FirstOrDefaultAsync(a => a.AccountID == accountId);

            if (account == null || account.IsDeleted)

                return NotFound();

            if (account.Customer.ApplicationUserID != userId)

                return Forbid();

            var result = await _transactionRepository.GetTransactionsByAccountId(accountId);

            return Ok(result);

        }


        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingTransactions()
        {
            var result = await _transactionRepository.GetPendingTransactions();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var result = await _transactionRepository.GetTransactionById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transaction)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _transactionRepository.CreateTransaction(transaction, userId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveTransaction(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _transactionRepository.ApproveTransaction(id, userId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectTransaction(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _transactionRepository.RejectTransaction(id, userId);
            return Ok(result);
        }
    }

}


