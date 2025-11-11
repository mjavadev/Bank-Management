using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{
   // [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var result = await _employeeRepository.GetAllEmployees();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var result = await _employeeRepository.GetEmployeeById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto employee)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _employeeRepository.CreateEmployee(employee, userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDto employee)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _employeeRepository.UpdateEmployee(id, employee, userId);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _employeeRepository.DeleteEmployee(id, userId);
            return Ok(result);
        }
    }

}
