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

        public async Task<Result<List<CustomerDto>>> GetAllCustomers()
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
        }

        public async Task<Result<CustomerDto>> GetCustomerById(int id)
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
                        IsActive = c.IsActive
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
        }

        public async Task<Result<CustomerDto>> GetCustomerByUserId(string userId)
        {
            Result<CustomerDto> response = new();

            try
            {
                var customer = await _context.Customers
                    .Where(c => c.ApplicationUserID == userId && !c.IsDeleted)
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
        }

        public async Task<Result<bool>> UpdateCustomer(CustomerDto customerDto, string modifiedBy)
        {
            Result<bool> response = new();

            try
            {
                var customer = await _context.Customers.FindAsync(customerDto.CustomerID);
                if (customer == null || customer.IsDeleted)
                {
                    response.Errors.Add(new Errors { ErrorCode = "202", ErrorMessage = "Customer not found" });
                    return response;
                }

                customer.DateOfBirth = customerDto.DateOfBirth;
                customer.Gender = customerDto.Gender;
                customer.Occupation = customerDto.Occupation;
                customer.MobileNumber = customerDto.MobileNumber;
                customer.AadharNumber = customerDto.AadharNumber;
                customer.PAN = customerDto.PAN;
                customer.ModifiedBy = modifiedBy;
                customer.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "201", ErrorMessage = ex.Message });
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



