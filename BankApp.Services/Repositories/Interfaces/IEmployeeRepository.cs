using BankApp.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Services.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Result<List<EmployeeDto>>> GetAllEmployees();
        Task<Result<EmployeeDto>> GetEmployeeById(int id);
        Task<Result<EmployeeDto>> CreateEmployee(EmployeeDto employee, string createdBy);
        Task<Result<bool>> UpdateEmployee(EmployeeDto employee, string modifiedBy);
        Task<Result<bool>> DeleteEmployee(int id, string deletedBy);
    }
}
