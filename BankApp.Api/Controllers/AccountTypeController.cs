using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTypeController : ControllerBase
    {
        private readonly IAccountTypeRepository _repository;
        public AccountTypeController(IAccountTypeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountTypes()
        {
            var types = await _repository.GetAllAccountTypesAsync();
            return Ok(types);
        }
    }

}
