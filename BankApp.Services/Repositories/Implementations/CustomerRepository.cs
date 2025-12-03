using BankApp.Entity.Dto;
using BankApp.Entity.Security;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.BankingApi.Entity.Models;

namespace BankApp.Services.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /* public async Task<Result<List<CustomerDto>>> GetAllCustomers()
         {
             Result<List<CustomerDto>> response = new();

             try
             {
                 var customers = await _context.Customers
                     .Where(c => !c.IsDeleted)
                     .Include(c => c.ApplicationUser)
                     .Include(c => c.ApprovedByUser)
                     .Select(c => new CustomerDto
                     {
                         CustomerID = c.CustomerID,
                         ApplicationUserID = c.ApplicationUserID,
                         UserName = c.ApplicationUser.UserName,
                         FullName = c.ApplicationUser.FullName,
                         DateOfBirth = c.DateOfBirth,
                         Gender = c.Gender,
                         Occupation = c.Occupation,
                         MobileNumber = c.MobileNumber,
                         ApprovedByUserID = c.ApprovedByUserID,
                         ApprovedByName = c.ApprovedByUser != null ? c.ApprovedByUser.FullName : null,
                         ApprovalDate = c.ApprovalDate,
                         AadharNumber = c.AadharNumber,
                         PAN = c.PAN,
                         CustomerImageURL = c.CustomerImageURL,
                         IsActive = c.IsActive
                     })
                     .ToListAsync();

                 response.Response = customers;
             }
             catch (Exception ex)
             {
                 response.Errors.Add(new Errors { ErrorCode = "201", ErrorMessage = ex.Message });
             }

             return response;
         }*/
        public async Task<Result<List<CustomerDto>>> GetAllCustomers()
        {
            Result<List<CustomerDto>> response = new();

            try
            {
                // Step 1: Load entities with all related data
                var customers = await _context.Customers
                    .Where(c => !c.IsDeleted)
                    .Include(c => c.ApplicationUser)
                    .Include(c => c.ApprovedByUser)
                    .Include(c => c.Accounts)  // ← Include accounts
                        .ThenInclude(a => a.AccountType)  // ← Include account type
                    .ToListAsync();

                // Step 2: Map to DTO list after fetching
                var customerDtos = customers.Select(c => new CustomerDto
                {
                    CustomerID = c.CustomerID,
                    ApplicationUserID = c.ApplicationUserID,
                    UserName = c.ApplicationUser.UserName,
                    FullName = c.ApplicationUser.FullName,
                    DateOfBirth = c.DateOfBirth,
                    Gender = c.Gender,
                    Occupation = c.Occupation,
                    MobileNumber = c.MobileNumber,
                    ApprovedByUserID = c.ApprovedByUserID,
                    ApprovedByName = c.ApprovedByUser != null ? c.ApprovedByUser.FullName : null,
                    ApprovalDate = c.ApprovalDate,
                    AadharNumber = c.AadharNumber,
                    PAN = c.PAN,
                    CustomerImageURL = c.CustomerImageURL,
                    IsActive = c.IsActive,
                    Accounts = c.Accounts
                        .Where(a => !a.IsDeleted)  // Only non-deleted accounts
                        .Select(a => new AccountDto
                        {
                            AccountID = a.AccountID,
                            AccountNumber = a.AccountNumber,
                            AccountTypeName = a.AccountType.TypeName,
                            Balance = a.Balance,
                            OpenDate = a.OpenDate,
                            Status = a.Status,
                            IsActive = a.IsActive
                        }).ToList()
                }).ToList();

                response.Response = customerDtos;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors
                {
                    ErrorCode = "201",
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }


        /* public async Task<Result<CustomerDto>> GetCustomerById(int id)
         {
             Result<CustomerDto> response = new();

             try
             {
                 var customer = await _context.Customers
                     .Where(c => c.CustomerID == id && !c.IsDeleted)
                     .Include(c => c.ApplicationUser)
                     .Include(c => c.ApprovedByUser)
                     .Select(c => new CustomerDto
                     {
                         CustomerID = c.CustomerID,
                         ApplicationUserID = c.ApplicationUserID,
                         UserName = c.ApplicationUser.UserName,
                         FullName = c.ApplicationUser.FullName,
                         DateOfBirth = c.DateOfBirth,
                         Gender = c.Gender,
                         Occupation = c.Occupation,
                         MobileNumber = c.MobileNumber,
                         ApprovedByUserID = c.ApprovedByUserID,
                         ApprovedByName = c.ApprovedByUser != null ? c.ApprovedByUser.FullName : null,
                         ApprovalDate = c.ApprovalDate,
                         AadharNumber = c.AadharNumber,
                         PAN = c.PAN,
                         CustomerImageURL = c.CustomerImageURL,
                         IsActive = c.IsActive,
                     })
                     .FirstOrDefaultAsync();

                 if (customer == null)
                 {
                     response.Errors.Add(new Errors { ErrorCode = "202", ErrorMessage = "Customer not found" });
                 }
                 else
                 {
                     response.Response = customer;
                 }
             }
             catch (Exception ex)
             {
                 response.Errors.Add(new Errors { ErrorCode = "201", ErrorMessage = ex.Message });
             }

             return response;
         }*/

        public async Task<Result<CustomerDto>> GetCustomerById(int id)
        {
            Result<CustomerDto> response = new();

            try
            {
                var customer = await _context.Customers
                    .Where(c => c.CustomerID == id && !c.IsDeleted)
                    .Include(c => c.ApplicationUser)
                    .Include(c => c.ApprovedByUser)
                    .Include(c => c.Accounts)  // ← Include accounts
                        .ThenInclude(a => a.AccountType)  // ← Include account type
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "202",
                        ErrorMessage = "Customer not found"
                    });
                    return response;
                }

                // Map to DTO after fetching
                var customerDto = new CustomerDto
                {
                    CustomerID = customer.CustomerID,
                    ApplicationUserID = customer.ApplicationUserID,
                    UserName = customer.ApplicationUser.UserName,
                    FullName = customer.ApplicationUser.FullName,
                    DateOfBirth = customer.DateOfBirth,
                    Gender = customer.Gender,
                    Occupation = customer.Occupation,
                    MobileNumber = customer.MobileNumber,
                    ApprovedByUserID = customer.ApprovedByUserID,
                    ApprovedByName = customer.ApprovedByUser != null ? customer.ApprovedByUser.FullName : null,
                    ApprovalDate = customer.ApprovalDate,
                    AadharNumber = customer.AadharNumber,
                    PAN = customer.PAN,
                    CustomerImageURL = customer.CustomerImageURL,
                    IsActive = customer.IsActive,
                    Accounts = customer.Accounts
                        .Where(a => !a.IsDeleted)  // only include non-deleted accounts
                        .Select(a => new AccountDto
                        {
                            AccountID = a.AccountID,
                            AccountNumber = a.AccountNumber,
                            AccountTypeName = a.AccountType.TypeName,
                            Balance = a.Balance,
                            OpenDate = a.OpenDate,
                            Status = a.Status,
                            IsActive = a.IsActive
                        }).ToList()
                };

                response.Response = customerDto;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors
                {
                    ErrorCode = "201",
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }

        public async Task<Result<CustomerDto>> GetCustomerByUserId(string userId)
        {
            Result<CustomerDto> response = new();

            try
            {
                // Step 1: Load entity with all related data
                var customer = await _context.Customers
                    .Where(c => c.ApplicationUserID == userId && !c.IsDeleted)
                    .Include(c => c.ApplicationUser)
                    .Include(c => c.ApprovedByUser)
                    .Include(c => c.Accounts)  // ← Include accounts
                        .ThenInclude(a => a.AccountType)  // ← Include account type
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "202",
                        ErrorMessage = "Customer not found"
                    });
                    return response;
                }

                // Step 2: Map to DTO after fetching
                var customerDto = new CustomerDto
                {
                    CustomerID = customer.CustomerID,
                    ApplicationUserID = customer.ApplicationUserID,
                    UserName = customer.ApplicationUser.UserName,
                    FullName = customer.ApplicationUser.FullName,
                    DateOfBirth = customer.DateOfBirth,
                    Gender = customer.Gender,
                    Occupation = customer.Occupation,
                    MobileNumber = customer.MobileNumber,
                    ApprovedByUserID = customer.ApprovedByUserID,
                    ApprovedByName = customer.ApprovedByUser != null ? customer.ApprovedByUser.FullName : null,
                    ApprovalDate = customer.ApprovalDate,
                    AadharNumber = customer.AadharNumber,
                    PAN = customer.PAN,
                    CustomerImageURL = customer.CustomerImageURL,
                    IsActive = customer.IsActive,
                    Accounts = customer.Accounts
                        .Where(a => !a.IsDeleted)  // Only non-deleted accounts
                        .Select(a => new AccountDto
                        {
                            AccountID = a.AccountID,
                            AccountNumber = a.AccountNumber,
                            AccountTypeName = a.AccountType.TypeName,
                            Balance = a.Balance,
                            OpenDate = a.OpenDate,
                            Status = a.Status,
                            IsActive = a.IsActive
                        }).ToList()
                };

                response.Response = customerDto;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors
                {
                    ErrorCode = "201",
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }

        public async Task<Result<bool>> UpdateCustomer(int id, CustomerDto customerDto, string modifiedBy)
        {
            Result<bool> response = new();

            try
            {
                // Validate that ID in route matches ID in DTO
                if (id != customerDto.CustomerID)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "203",
                        ErrorMessage = "Customer ID mismatch"
                    });
                    return response;
                }

                // Load customer WITH ApplicationUser navigation property
                var customer = await _context.Customers
                    .Include(c => c.ApplicationUser)
                    .FirstOrDefaultAsync(c => c.CustomerID == id);

                if (customer == null || customer.IsDeleted)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "202",
                        ErrorMessage = "Customer not found"
                    });
                    return response;
                }

                // Update Customer fields
                customer.DateOfBirth = customerDto.DateOfBirth;
                customer.Gender = customerDto.Gender;
                customer.Occupation = customerDto.Occupation;
                customer.MobileNumber = customerDto.MobileNumber;
                customer.AadharNumber = customerDto.AadharNumber;
                customer.PAN = customerDto.PAN;
                customer.ModifiedBy = modifiedBy;
                customer.ModifiedDate = DateTime.Now;

                // Update ApplicationUser.FullName if provided
                if (!string.IsNullOrEmpty(customerDto.FullName))
                {
                    customer.ApplicationUser.FullName = customerDto.FullName;
                }

                // Save both Customer and ApplicationUser changes
                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors
                {
                    ErrorCode = "201",
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }


        public async Task<Result<bool>> DeleteCustomer(int id, string deletedBy)
        {
            Result<bool> response = new();

            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "202", ErrorMessage = "Customer not found" });
                    return response;
                }

                customer.IsDeleted = true;
                customer.DeletedBy = deletedBy;
                customer.DeletedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "201", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<bool>> DeactivateCustomer(int id, string deletedBy)
        {
            Result<bool> response = new();

            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Accounts)
                    .FirstOrDefaultAsync(c => c.CustomerID == id);

                if (customer == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "202", ErrorMessage = "Customer not found" });
                    return response;
                }

                // Deactivate customer
                customer.IsActive = false;
                customer.IsDeleted = true;
                customer.DeletedBy = deletedBy;
                customer.DeletedDate = DateTime.Now;

                // Deactivate all customer accounts
                foreach (var account in customer.Accounts.Where(a => !a.IsDeleted))
                {
                    account.IsActive = false;
                    account.IsDeleted = true;
                    account.DeletedBy = deletedBy;
                    account.DeletedDate = DateTime.Now;
                    account.Status = "Inactive";
                }

                // Deactivate user in AspNetUsers
                var user = await _userManager.FindByIdAsync(customer.ApplicationUserID);
                if (user != null)
                {
                    user.IsActive = false;
                    await _userManager.UpdateAsync(user);
                }

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "201", ErrorMessage = ex.Message });
            }

            return response;
        }
    }
}



