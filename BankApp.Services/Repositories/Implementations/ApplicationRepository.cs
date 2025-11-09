using BankApp.Entity.Dto;
using BankApp.Entity.Enums;
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
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Result<List<ApplicationDto>>> GetAllApplications()
        {
            Result<List<ApplicationDto>> response = new();

            try
            {
                var applications = await _context.CustomerApplications
                    .Where(a => !a.IsDeleted)
                    .Select(a => new ApplicationDto
                    {
                        ApplicationID = a.ApplicationID,
                        FullName = a.FullName,
                        DateOfBirth = a.DateOfBirth,
                        Occupation = a.Occupation,
                        AadharNumber = a.AadharNumber,
                        PAN = a.PAN,
                        CustomerImageURL = a.CustomerImageURL,
                        Status = a.Status,
                        ApplicationDate = a.ApplicationDate,
                        ApprovalDate = a.ApprovalDate,
                        RejectionReason = a.RejectionReason
                    })
                    .ToListAsync();

                response.Response = applications;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<List<ApplicationDto>>> GetPendingApplications()
        {
            Result<List<ApplicationDto>> response = new();

            try
            {
                var applications = await _context.CustomerApplications
                    .Where(a => !a.IsDeleted && a.Status == ApplicationStatus.Pending)
                    .Select(a => new ApplicationDto
                    {
                        ApplicationID = a.ApplicationID,
                        FullName = a.FullName,
                        DateOfBirth = a.DateOfBirth,
                        Occupation = a.Occupation,
                        AadharNumber = a.AadharNumber,
                        PAN = a.PAN,
                        CustomerImageURL = a.CustomerImageURL,
                        Status = a.Status,
                        ApplicationDate = a.ApplicationDate
                    })
                    .ToListAsync();

                response.Response = applications;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<ApplicationDto>> GetApplicationById(int id)
        {
            Result<ApplicationDto> response = new();

            try
            {
                var application = await _context.CustomerApplications
                    .Where(a => a.ApplicationID == id && !a.IsDeleted)
                    .Select(a => new ApplicationDto
                    {
                        ApplicationID = a.ApplicationID,
                        FullName = a.FullName,
                        DateOfBirth = a.DateOfBirth,
                        Occupation = a.Occupation,
                        AadharNumber = a.AadharNumber,
                        PAN = a.PAN,
                        CustomerImageURL = a.CustomerImageURL,
                        Status = a.Status,
                        ApplicationDate = a.ApplicationDate,
                        ApprovalDate = a.ApprovalDate,
                        RejectionReason = a.RejectionReason
                    })
                    .FirstOrDefaultAsync();

                if (application == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "402", ErrorMessage = "Application not found" });
                }
                else
                {
                    response.Response = application;
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<ApplicationDto>> CreateApplication(ApplicationDto applicationDto)
        {
            Result<ApplicationDto> response = new();

            try
            {
                var application = new CustomerApplication
                {
                    FullName = applicationDto.FullName,
                    DateOfBirth = applicationDto.DateOfBirth,
                    Occupation = applicationDto.Occupation,
                    AadharNumber = applicationDto.AadharNumber,
                    PAN = applicationDto.PAN,
                    CustomerImageURL = applicationDto.CustomerImageURL,
                    Status = ApplicationStatus.Pending,
                    ApplicationDate = DateTime.Now
                };

                _context.CustomerApplications.Add(application);
                await _context.SaveChangesAsync();

                applicationDto.ApplicationID = application.ApplicationID;
                applicationDto.Status = application.Status;
                applicationDto.ApplicationDate = application.ApplicationDate;
                response.Response = applicationDto;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<UserResponse>> ApproveApplication(int applicationId, string approvedBy)
        {
            Result<UserResponse> response = new();

            try
            {
                var application = await _context.CustomerApplications.FindAsync(applicationId);
                if (application == null || application.IsDeleted)
                {
                    response.Errors.Add(new Errors { ErrorCode = "402", ErrorMessage = "Application not found" });
                    return response;
                }

                if (application.Status != ApplicationStatus.Pending)
                {
                    response.Errors.Add(new Errors { ErrorCode = "403", ErrorMessage = "Application already processed" });
                    return response;
                }

                var maxCustomerId = await _context.Customers.MaxAsync(c => (int?)c.CustomerID) ?? 49999;
                var newCustomerId = maxCustomerId + 1;

                var namePart = new string(application.FullName.Where(char.IsLetterOrDigit).Take(4).ToArray()).ToLower();
                var idPart = newCustomerId.ToString().Substring(Math.Max(0, newCustomerId.ToString().Length - 4));
                var username = namePart + idPart;

                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = $"{username}@customer.com",
                    FullName = application.FullName,
                    IsActive = true,
                    MustChangePassword = true,
                    CreatedBy = approvedBy,
                    CreatedDate = DateTime.Now
                };

                var defaultPassword = "Default@123";
                var result = await _userManager.CreateAsync(user, defaultPassword);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        response.Errors.Add(new Errors { ErrorCode = "404", ErrorMessage = err.Description });
                    }
                    return response;
                }

                await _userManager.AddToRoleAsync(user, "Customer");

                var customer = new Customer
                {
                    CustomerID = newCustomerId,
                    ApplicationUserID = user.Id,
                    DateOfBirth = application.DateOfBirth,
                    Occupation = application.Occupation,
                    ApprovedByUserID = approvedBy,
                    ApprovalDate = DateTime.Now,
                    AadharNumber = application.AadharNumber,
                    PAN = application.PAN,
                    CustomerImageURL = application.CustomerImageURL,
                    CreatedBy = approvedBy,
                    CreatedDate = DateTime.Now
                };

                _context.Customers.Add(customer);

                application.Status = ApplicationStatus.Approved;
                application.ApprovedByUserID = approvedBy;
                application.ApprovalDate = DateTime.Now;

                await _context.SaveChangesAsync();

                response.Response = new UserResponse
                {
                    Id = user.Id,
                    UserName = username,
                    FullName = user.FullName,
                    Roles = new List<string> { "Customer" }
                };
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<bool>> RejectApplication(int applicationId, string rejectedBy, string reason)
        {
            Result<bool> response = new();

            try
            {
                var application = await _context.CustomerApplications.FindAsync(applicationId);
                if (application == null || application.IsDeleted)
                {
                    response.Errors.Add(new Errors { ErrorCode = "402", ErrorMessage = "Application not found" });
                    return response;
                }

                if (application.Status != ApplicationStatus.Pending)
                {
                    response.Errors.Add(new Errors { ErrorCode = "403", ErrorMessage = "Application already processed" });
                    return response;
                }

                application.Status = ApplicationStatus.Rejected;
                application.ApprovedByUserID = rejectedBy;
                application.ApprovalDate = DateTime.Now;
                application.RejectionReason = reason;

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = ex.Message });
            }

            return response;
        }
    }

}

