using BankApp.Entity.Dto;
using BankApp.Entity.Models;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.BankingApi.Entity.Models;

namespace BankApp.Services.Repositories.Implementations
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public BankAccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<AccountDto>>> GetAccountsByCustomerId(int customerId)
        {
            Result<List<AccountDto>> response = new();

            try
            {
                var accounts = await _context.Accounts
                    .Where(a => a.CustomerID == customerId && !a.IsDeleted)
                    .Include(a => a.AccountType)
                    .Include(a => a.Customer)
                        .ThenInclude(c => c.ApplicationUser)
                    .Select(a => new AccountDto
                    {
                        AccountID = a.AccountID,
                        AccountNumber = a.AccountNumber,
                        CustomerID = a.CustomerID,
                        CustomerName = a.Customer.ApplicationUser.FullName,
                        AccountTypeID = a.AccountTypeID,
                        AccountTypeName = a.AccountType.TypeName,
                        Balance = a.Balance,
                        OpenDate = a.OpenDate,
                        Status = a.Status,
                        IsActive = a.IsActive
                    })
                    .ToListAsync();

                response.Response = accounts;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "601", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<AccountDto>> GetAccountById(int id)
        {
            Result<AccountDto> response = new();

            try
            {
                var account = await _context.Accounts
                    .Where(a => a.AccountID == id && !a.IsDeleted)
                    .Include(a => a.AccountType)
                    .Include(a => a.Customer)
                        .ThenInclude(c => c.ApplicationUser)
                    .Select(a => new AccountDto
                    {
                        AccountID = a.AccountID,
                        AccountNumber = a.AccountNumber,
                        CustomerID = a.CustomerID,
                        CustomerName = a.Customer.ApplicationUser.FullName,
                        AccountTypeID = a.AccountTypeID,
                        AccountTypeName = a.AccountType.TypeName,
                        Balance = a.Balance,
                        OpenDate = a.OpenDate,
                        Status = a.Status,
                        IsActive = a.IsActive
                    })
                    .FirstOrDefaultAsync();

                if (account == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "602", ErrorMessage = "Account not found" });
                }
                else
                {
                    response.Response = account;
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "601", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<AccountDto>> CreateAccount(AccountDto accountDto, string createdBy)
        {
            Result<AccountDto> response = new();

            try
            {
                // Verify customer exists and is active
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerID == accountDto.CustomerID && !c.IsDeleted);

                if (customer == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "603", ErrorMessage = "Customer not found" });
                    return response;
                }

                if (!customer.IsActive)
                {
                    response.Errors.Add(new Errors { ErrorCode = "604", ErrorMessage = "Customer is inactive" });
                    return response;
                }

                // Check if customer already has this account type
                var existingAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.CustomerID == accountDto.CustomerID
                                           && a.AccountTypeID == accountDto.AccountTypeID
                                           && !a.IsDeleted);

                if (existingAccount != null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "605", ErrorMessage = "Customer already has this account type" });
                    return response;
                }

                // Generate Account Number
                var maxAccountNumber = await _context.Accounts
                    .IgnoreQueryFilters()
                    .MaxAsync(a => (long?)Convert.ToInt64(a.AccountNumber)) ?? 88855999;

                var newAccountNumber = (maxAccountNumber + 1).ToString();

                // Create Account
                var account = new Account
                {
                    AccountNumber = newAccountNumber,
                    CustomerID = accountDto.CustomerID,
                    AccountTypeID = accountDto.AccountTypeID,
                    Balance = 0,
                    OpenDate = DateTime.Now,
                    Status = "Active",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                accountDto.AccountID = account.AccountID;
                accountDto.AccountNumber = account.AccountNumber;
                accountDto.Balance = account.Balance;
                accountDto.OpenDate = account.OpenDate;
                accountDto.Status = account.Status;
                accountDto.IsActive = account.IsActive;

                response.Response = accountDto;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "601", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<bool>> DeactivateAccount(int id, string deletedBy)
        {
            Result<bool> response = new();

            try
            {
                var account = await _context.Accounts.FindAsync(id);
                if (account == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "602", ErrorMessage = "Account not found" });
                    return response;
                }

                account.IsActive = false;
                account.IsDeleted = true;
                account.DeletedBy = deletedBy;
                account.DeletedDate = DateTime.Now;
                account.Status = "Inactive";

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "601", ErrorMessage = ex.Message });
            }

            return response;
        }
    }

}

