using BankApp.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Services.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Result<List<CustomerDto>>> GetAllCustomers();
        Task<Result<CustomerDto>> GetCustomerById(int id);
        Task<Result<CustomerDto>> GetCustomerByUserId(string userId);
        Task<Result<bool>> UpdateCustomer(int id, CustomerDto customer, string modifiedBy);
        Task<Result<bool>> DeleteCustomer(int id, string deletedBy);
        Task<Result<bool>> DeactivateCustomer(int id, string deletedBy);
    }
}
