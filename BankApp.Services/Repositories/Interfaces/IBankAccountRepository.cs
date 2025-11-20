using BankApp.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Services.Repositories.Interfaces
{
    public interface IBankAccountRepository
    {
        Task<Result<List<AccountDto>>> GetAccountsByCustomerId(int customerId);
        Task<Result<AccountDto>> GetAccountById(int id);
        Task<Result<AccountDto>> CreateAccount(AccountDto accountDto, string createdBy);
        Task<Result<bool>> DeactivateAccount(int id, string deletedBy);

    }

}

