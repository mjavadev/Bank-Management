using BankApp.Entity.Dto;
using BankApp.Entity.Models;
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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Result<List<EmployeeDto>>> GetAllEmployees()
        {
            Result<List<EmployeeDto>> response = new();

            try
            {
                var employees = await _context.Employees
                    .Where(e => !e.IsDeleted)
                    .Include(e => e.ApplicationUser)
                    .Select(e => new EmployeeDto
                    {
                        EmployeeID = e.EmployeeID,
                        ApplicationUserID = e.ApplicationUserID,
                        UserName = e.ApplicationUser.UserName,
                        FullName = e.ApplicationUser.FullName,
                        StaffCode = e.StaffCode,
                        JobTitle = e.JobTitle,
                        DateHired = e.DateHired
                    })
                    .ToListAsync();

                response.Response = employees;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "301", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<EmployeeDto>> GetEmployeeById(int id)
        {
            Result<EmployeeDto> response = new();

            try
            {
                var employee = await _context.Employees
                    .Where(e => e.EmployeeID == id && !e.IsDeleted)
                    .Include(e => e.ApplicationUser)
                    .Select(e => new EmployeeDto
                    {
                        EmployeeID = e.EmployeeID,
                        ApplicationUserID = e.ApplicationUserID,
                        UserName = e.ApplicationUser.UserName,
                        FullName = e.ApplicationUser.FullName,
                        StaffCode = e.StaffCode,
                        JobTitle = e.JobTitle,
                        DateHired = e.DateHired
                    })
                    .FirstOrDefaultAsync();

                if (employee == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "302", ErrorMessage = "Employee not found" });
                }
                else
                {
                    response.Response = employee;
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "301", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<EmployeeDto>> CreateEmployee(EmployeeDto employeeDto, string createdBy)
        {
            Result<EmployeeDto> response = new();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = employeeDto.UserName,
                    Email = employeeDto.UserName,
                    FullName = employeeDto.FullName,
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, employeeDto.Password ?? "Default@123");
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        response.Errors.Add(new Errors { ErrorCode = "303", ErrorMessage = err.Description });
                    }
                    return response;
                }

                await _userManager.AddToRoleAsync(user, "Manager");

                var employee = new Employee
                {
                    ApplicationUserID = user.Id,
                    StaffCode = employeeDto.StaffCode,
                    JobTitle = employeeDto.JobTitle,
                    DateHired = employeeDto.DateHired,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                employeeDto.EmployeeID = employee.EmployeeID;
                employeeDto.ApplicationUserID = user.Id;
                response.Response = employeeDto;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "301", ErrorMessage = ex.Message });
            }

            return response;
        }

        //public async Task<Result<bool>> UpdateEmployee(EmployeeDto employeeDto, string modifiedBy)
        //{
        //    Result<bool> response = new();

        //    try
        //    {
        //        var employee = await _context.Employees.FindAsync(employeeDto.EmployeeID);
        //        if (employee == null || employee.IsDeleted)
        //        {
        //            response.Errors.Add(new Errors { ErrorCode = "302", ErrorMessage = "Employee not found" });
        //            return response;
        //        }

        //        employee.StaffCode = employeeDto.StaffCode;
        //        employee.JobTitle = employeeDto.JobTitle;
        //        employee.DateHired = employeeDto.DateHired;
        //        employee.ModifiedBy = modifiedBy;
        //        employee.ModifiedDate = DateTime.Now;

        //        await _context.SaveChangesAsync();
        //        response.Response = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(new Errors { ErrorCode = "301", ErrorMessage = ex.Message });
        //    }

        //    return response;
        //}

        public async Task<Result<bool>> UpdateEmployee(int id, EmployeeDto employeeDto, string modifiedBy)
        {
            Result<bool> response = new();

            try
            {
                // Validate that ID in route matches ID in DTO (optional but recommended)
                if (id != employeeDto.EmployeeID)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "303",
                        ErrorMessage = "Employee ID mismatch"
                    });
                    return response;
                }

                var employee = await _context.Employees.FindAsync(id);
                if (employee == null || employee.IsDeleted)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "302",
                        ErrorMessage = "Employee not found"
                    });
                    return response;
                }

                var employeeData = await _context.Employees
                .Include(e => e.ApplicationUser) 
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

                // Update ApplicationUser.FullName with simple validation
                if (!string.IsNullOrWhiteSpace(employeeDto.FullName))
                {
                    var trimmedName = employeeDto.FullName.Trim();

                    if (trimmedName.Length >= 2 && trimmedName.Length <= 100)
                    {
                        employee.ApplicationUser.FullName = trimmedName;
                    }
                    else
                    {
                        response.Errors.Add(new Errors
                        {
                            ErrorCode = "304",
                            ErrorMessage = "Full name must be between 2 and 100 characters"
                        });
                        return response;
                    }
                }

                employee.StaffCode = employeeDto.StaffCode;
                employee.JobTitle = employeeDto.JobTitle;
                employee.DateHired = employeeDto.DateHired;
                employee.ModifiedBy = modifiedBy;
                employee.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors
                {
                    ErrorCode = "301",
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }


        public async Task<Result<bool>> DeleteEmployee(int id, string deletedBy)
        {
            Result<bool> response = new();

            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "302", ErrorMessage = "Employee not found" });
                    return response;
                }

                employee.IsDeleted = true;
                employee.DeletedBy = deletedBy;
                employee.DeletedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "301", ErrorMessage = ex.Message });
            }

            return response;
        }
    }

}

