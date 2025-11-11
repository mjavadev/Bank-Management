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
                        Gender = a.Gender,
                        Occupation = a.Occupation,
                        MobileNumber = a.MobileNumber,
                        AadharNumber = a.AadharNumber,
                        PAN = a.PAN,
                        CustomerImageURL = a.CustomerImageURL,
                        AccountTypeID = a.AccountTypeID,
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
                        Gender = a.Gender,
                        Occupation = a.Occupation,
                        MobileNumber = a.MobileNumber,
                        AadharNumber = a.AadharNumber,
                        PAN = a.PAN,
                        CustomerImageURL = a.CustomerImageURL,
                        AccountTypeID = a.AccountTypeID,
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
                        Gender = a.Gender,
                        Occupation = a.Occupation,
                        MobileNumber = a.MobileNumber,
                        AadharNumber = a.AadharNumber,
                        PAN = a.PAN,
                        CustomerImageURL = a.CustomerImageURL,
                        AccountTypeID = a.AccountTypeID,
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
                // VALIDATION 1: Check if Mobile Number already exists
                var mobileExists = await _context.Customers
                    .AnyAsync(c => c.MobileNumber == applicationDto.MobileNumber && !c.IsDeleted);

                if (mobileExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "407",
                        ErrorMessage = "Mobile number already registered"
                    });
                    return response;
                }

                // Also check in pending applications
                var mobilePendingExists = await _context.CustomerApplications
                    .AnyAsync(a => a.MobileNumber == applicationDto.MobileNumber
                                && a.Status == ApplicationStatus.Pending
                                && !a.IsDeleted);

                if (mobilePendingExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "408",
                        ErrorMessage = "Mobile number already in pending applications"
                    });
                    return response;
                }

                // VALIDATION 2: Check if Aadhar Number already exists
                var aadharExists = await _context.Customers
                    .AnyAsync(c => c.AadharNumber == applicationDto.AadharNumber && !c.IsDeleted);

                if (aadharExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "409",
                        ErrorMessage = "Aadhar number already registered"
                    });
                    return response;
                }

                // Also check in pending applications
                var aadharPendingExists = await _context.CustomerApplications
                    .AnyAsync(a => a.AadharNumber == applicationDto.AadharNumber
                                && a.Status == ApplicationStatus.Pending
                                && !a.IsDeleted);

                if (aadharPendingExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "410",
                        ErrorMessage = "Aadhar number already in pending applications"
                    });
                    return response;
                }

                // VALIDATION 3: Check if PAN already exists
                var panExists = await _context.Customers
                    .AnyAsync(c => c.PAN == applicationDto.PAN && !c.IsDeleted);

                if (panExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "411",
                        ErrorMessage = "PAN already registered"
                    });
                    return response;
                }

                // Also check in pending applications
                var panPendingExists = await _context.CustomerApplications
                    .AnyAsync(a => a.PAN == applicationDto.PAN
                                && a.Status == ApplicationStatus.Pending
                                && !a.IsDeleted);

                if (panPendingExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "412",
                        ErrorMessage = "PAN already in pending applications"
                    });
                    return response;
                }

                var application = new CustomerApplication
                {
                    FullName = applicationDto.FullName,
                    DateOfBirth = applicationDto.DateOfBirth,
                    Gender = applicationDto.Gender,
                    Occupation = applicationDto.Occupation,
                    MobileNumber = applicationDto.MobileNumber,
                    AadharNumber = applicationDto.AadharNumber,
                    PAN = applicationDto.PAN,
                    CustomerImageURL = applicationDto.CustomerImageURL,
                    AccountTypeID = applicationDto.AccountTypeID,
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

        //public async Task<Result<UserResponse>> ApproveApplication(int applicationId, string approvedBy)
        //{
        //    Result<UserResponse> response = new();

        //    try
        //    {
        //        var application = await _context.CustomerApplications.FindAsync(applicationId);
        //        if (application == null || application.IsDeleted)
        //        {
        //            response.Errors.Add(new Errors { ErrorCode = "402", ErrorMessage = "Application not found" });
        //            return response;
        //        }

        //        if (application.Status != ApplicationStatus.Pending)
        //        {
        //            response.Errors.Add(new Errors { ErrorCode = "403", ErrorMessage = "Application already processed" });
        //            return response;
        //        }

        //        var maxCustomerId = await _context.Customers.MaxAsync(c => (int?)c.CustomerID) ?? 49999;
        //        var newCustomerId = maxCustomerId + 1;

        //        var namePart = new string(application.FullName.Where(char.IsLetterOrDigit).Take(4).ToArray()).ToLower();
        //        var idPart = newCustomerId.ToString().Substring(Math.Max(0, newCustomerId.ToString().Length - 4));
        //        var username = namePart + idPart;

        //        var user = new ApplicationUser
        //        {
        //            UserName = username,
        //            Email = $"{username}@customer.com",
        //            FullName = application.FullName,
        //            IsActive = true,
        //            MustChangePassword = true,
        //            CreatedBy = approvedBy,
        //            CreatedDate = DateTime.Now
        //        };

        //        var defaultPassword = GenerateRandomPassword();
        //        var result = await _userManager.CreateAsync(user, defaultPassword);
        //        if (!result.Succeeded)
        //        {
        //            foreach (var err in result.Errors)
        //            {
        //                response.Errors.Add(new Errors { ErrorCode = "404", ErrorMessage = err.Description });
        //            }
        //            return response;
        //        }

        //        await _userManager.AddToRoleAsync(user, "Customer");

        //        var customer = new Customer
        //        {
        //            CustomerID = newCustomerId,
        //            ApplicationUserID = user.Id,
        //            DateOfBirth = application.DateOfBirth,
        //            Occupation = application.Occupation,
        //            ApprovedByUserID = approvedBy,
        //            ApprovalDate = DateTime.Now,
        //            AadharNumber = application.AadharNumber,
        //            PAN = application.PAN,
        //            CustomerImageURL = application.CustomerImageURL,
        //            CreatedBy = approvedBy,
        //            CreatedDate = DateTime.Now
        //        };

        //        _context.Customers.Add(customer);

        //        application.Status = ApplicationStatus.Approved;
        //        application.ApprovedByUserID = approvedBy;
        //        application.ApprovalDate = DateTime.Now;

        //        await _context.SaveChangesAsync();

        //        response.Response = new UserResponse
        //        {
        //            Id = user.Id,
        //            UserName = username,
        //            FullName = user.FullName,
        //            Roles = new List<string> { "Customer" },
        //            TemporaryPassword = defaultPassword
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = ex.Message });
        //    }

        //    return response;
        //}
        //public async Task<Result<UserResponse>> ApproveApplication(int applicationId, string approvedBy)

        //{

        //    Result<UserResponse> response = new();

        //    // Use database transaction to ensure atomicity

        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try

        //    {

        //        var application = await _context.CustomerApplications.FindAsync(applicationId);

        //        if (application == null || application.IsDeleted)

        //        {

        //            response.Errors.Add(new Errors { ErrorCode = "402", ErrorMessage = "Application not found" });

        //            return response;

        //        }

        //        if (application.Status != ApplicationStatus.Pending)

        //        {

        //            response.Errors.Add(new Errors { ErrorCode = "403", ErrorMessage = "Application already processed" });

        //            return response;

        //        }

        //        // Get next CustomerID (starting from 50000)

        //        var maxCustomerId = await _context.Customers.MaxAsync(c => (int?)c.CustomerID) ?? 49999;

        //        var newCustomerId = maxCustomerId + 1;

        //        // Generate username: first 4 chars of name + last 4 digits of customer ID

        //        var namePart = new string(application.FullName.Where(char.IsLetterOrDigit).Take(4).ToArray()).ToLower();

        //        var idPart = newCustomerId.ToString().PadLeft(4, '0').Substring(Math.Max(0, newCustomerId.ToString().Length - 4));

        //        var username = namePart + idPart;

        //        // Check if username already exists

        //        var existingUser = await _userManager.FindByNameAsync(username);

        //        if (existingUser != null)

        //        {

        //            response.Errors.Add(new Errors { ErrorCode = "405", ErrorMessage = "Username already exists" });

        //            await transaction.RollbackAsync();

        //            return response;

        //        }

        //        // Create ApplicationUser

        //        var user = new ApplicationUser

        //        {

        //            UserName = username,

        //            Email = $"{username}@customer.com",

        //            FullName = application.FullName,

        //            IsActive = true,

        //            MustChangePassword = true,

        //            CreatedBy = approvedBy,

        //            CreatedDate = DateTime.Now

        //        };

        //        var defaultPassword = "Default@123";

        //        var userResult = await _userManager.CreateAsync(user, defaultPassword);

        //        if (!userResult.Succeeded)

        //        {

        //            foreach (var err in userResult.Errors)

        //            {

        //                response.Errors.Add(new Errors { ErrorCode = "404", ErrorMessage = err.Description });

        //            }

        //            await transaction.RollbackAsync();

        //            return response;

        //        }

        //        // Assign Customer role

        //        var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

        //        if (!roleResult.Succeeded)

        //        {

        //            foreach (var err in roleResult.Errors)

        //            {

        //                response.Errors.Add(new Errors { ErrorCode = "406", ErrorMessage = err.Description });

        //            }

        //            await transaction.RollbackAsync();

        //            return response;

        //        }

        //        // Create Customer record

        //        var customer = new Customer

        //        {

        //            CustomerID = newCustomerId,

        //            ApplicationUserID = user.Id,

        //            DateOfBirth = application.DateOfBirth,

        //            Occupation = application.Occupation,

        //            ApprovedByUserID = approvedBy,

        //            ApprovalDate = DateTime.Now,

        //            AadharNumber = application.AadharNumber,

        //            PAN = application.PAN,

        //            CustomerImageURL = application.CustomerImageURL,

        //            CreatedBy = approvedBy,

        //            CreatedDate = DateTime.Now

        //        };

        //        _context.Customers.Add(customer);

        //        // Update application status

        //        application.Status = ApplicationStatus.Approved;

        //        application.ApprovedByUserID = approvedBy;

        //        application.ApprovalDate = DateTime.Now;

        //        application.ModifiedBy = approvedBy;

        //        application.ModifiedDate = DateTime.Now;

        //        // Save all changes

        //        await _context.SaveChangesAsync();

        //        // Commit transaction - all changes succeed together

        //        await transaction.CommitAsync();

        //        response.Response = new UserResponse

        //        {

        //            Id = user.Id,

        //            UserName = username,

        //            FullName = user.FullName,

        //            Roles = new List<string> { "Customer" }

        //        };

        //    }

        //    catch (Exception ex)

        //    {
        //        await transaction.RollbackAsync();

        //        _context.ChangeTracker.Clear();

        //        response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = ex.Message });

        //    }

        //    return response;

        //}
        //updated to fix duplication of customer ids:

        public async Task<Result<bool>> UpdateApplication(int id, ApplicationDto applicationDto, string modifiedBy)
        {
            Result<bool> response = new();

            try
            {
                // Validate that ID in route matches ID in DTO
                if (id != applicationDto.ApplicationID)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "416",
                        ErrorMessage = "Application ID mismatch"
                    });
                    return response;
                }

                // Find the application
                var application = await _context.CustomerApplications.FindAsync(id);

                if (application == null || application.IsDeleted)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "402",
                        ErrorMessage = "Application not found"
                    });
                    return response;
                }

                // Only allow updates for Pending applications
                if (application.Status != ApplicationStatus.Pending)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "417",
                        ErrorMessage = "Only pending applications can be updated"
                    });
                    return response;
                }

                // Validate uniqueness of Mobile, Aadhar, PAN (excluding current application)
                var mobileExists = await _context.Customers
                    .AnyAsync(c => c.MobileNumber == applicationDto.MobileNumber && !c.IsDeleted);

                if (!mobileExists)
                {
                    mobileExists = await _context.CustomerApplications
                        .AnyAsync(a => a.MobileNumber == applicationDto.MobileNumber
                                    && a.ApplicationID != id
                                    && a.Status == ApplicationStatus.Pending
                                    && !a.IsDeleted);
                }

                if (mobileExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "418",
                        ErrorMessage = "Mobile number already registered"
                    });
                    return response;
                }

                var aadharExists = await _context.Customers
                    .AnyAsync(c => c.AadharNumber == applicationDto.AadharNumber && !c.IsDeleted);

                if (!aadharExists)
                {
                    aadharExists = await _context.CustomerApplications
                        .AnyAsync(a => a.AadharNumber == applicationDto.AadharNumber
                                    && a.ApplicationID != id
                                    && a.Status == ApplicationStatus.Pending
                                    && !a.IsDeleted);
                }

                if (aadharExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "419",
                        ErrorMessage = "Aadhar number already registered"
                    });
                    return response;
                }

                var panExists = await _context.Customers
                    .AnyAsync(c => c.PAN == applicationDto.PAN && !c.IsDeleted);

                if (!panExists)
                {
                    panExists = await _context.CustomerApplications
                        .AnyAsync(a => a.PAN == applicationDto.PAN
                                    && a.ApplicationID != id
                                    && a.Status == ApplicationStatus.Pending
                                    && !a.IsDeleted);
                }

                if (panExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "420",
                        ErrorMessage = "PAN already registered"
                    });
                    return response;
                }

                // Update application fields
                application.FullName = applicationDto.FullName;
                application.DateOfBirth = applicationDto.DateOfBirth;
                application.Gender = applicationDto.Gender;
                application.Occupation = applicationDto.Occupation;
                application.MobileNumber = applicationDto.MobileNumber;
                application.AadharNumber = applicationDto.AadharNumber;
                application.PAN = applicationDto.PAN;
                application.AccountTypeID = applicationDto.AccountTypeID;

                // Update image URL only if provided
                if (!string.IsNullOrEmpty(applicationDto.CustomerImageURL))
                {
                    application.CustomerImageURL = applicationDto.CustomerImageURL;
                }

                application.ModifiedBy = modifiedBy;
                application.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors
                {
                    ErrorCode = "401",
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }


        public async Task<Result<UserResponse>> ApproveApplication(int applicationId, string approvedBy)

        {

            Result<UserResponse> response = new();
            using var transaction = await _context.Database.BeginTransactionAsync();

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

                // RE-VALIDATE uniqueness before approval (in case duplicate was added while pending)
                var mobileExists = await _context.Customers
                    .AnyAsync(c => c.MobileNumber == application.MobileNumber && !c.IsDeleted);

                if (mobileExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "413",
                        ErrorMessage = "Mobile number already registered by another customer"
                    });
                    await transaction.RollbackAsync();
                    return response;
                }

                var aadharExists = await _context.Customers
                    .AnyAsync(c => c.AadharNumber == application.AadharNumber && !c.IsDeleted);

                if (aadharExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "414",
                        ErrorMessage = "Aadhar number already registered by another customer"
                    });
                    await transaction.RollbackAsync();
                    return response;
                }

                var panExists = await _context.Customers
                    .AnyAsync(c => c.PAN == application.PAN && !c.IsDeleted);

                if (panExists)
                {
                    response.Errors.Add(new Errors
                    {
                        ErrorCode = "415",
                        ErrorMessage = "PAN already registered by another customer"
                    });
                    await transaction.RollbackAsync();
                    return response;
                }

                var maxCustomerId = await _context.Customers
                    .IgnoreQueryFilters()
                    .MaxAsync(c => (int?)c.CustomerID) ?? 4999;

                var newCustomerId = maxCustomerId + 1;  

                var namePart = new string(application.FullName.Where(char.IsLetterOrDigit).Take(4).ToArray()).ToLower();

                if (namePart.Length < 4)
                {
                    namePart = namePart.PadRight(4, 'x');
                }

                var idPart = newCustomerId.ToString();

                var baseUsername = namePart + idPart;

                var username = baseUsername;

                var counter = 1;

                while (await _userManager.FindByNameAsync(username) != null)
                {
                    username = baseUsername + counter;
                    counter++;
                }

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

               // var defaultPassword = GenerateRandomPassword();
               var defaultPassword = "Default@123";

                var userResult = await _userManager.CreateAsync(user, defaultPassword);

                if (!userResult.Succeeded)
                {
                    foreach (var err in userResult.Errors)
                    {
                        response.Errors.Add(new Errors { ErrorCode = "404", ErrorMessage = err.Description });
                    }

                    await transaction.RollbackAsync();
                    return response;

                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

                if (!roleResult.Succeeded)
                {
                    foreach (var err in roleResult.Errors)
                    {
                        response.Errors.Add(new Errors { ErrorCode = "406", ErrorMessage = err.Description });
                    }

                    await transaction.RollbackAsync();
                    return response;
                }

                var customer = new Customer
                {
                    CustomerID = newCustomerId,
                    ApplicationUserID = user.Id,
                    DateOfBirth = application.DateOfBirth,
                    Gender = application.Gender,
                    Occupation = application.Occupation,
                    MobileNumber = application.MobileNumber,
                    ApprovedByUserID = approvedBy,
                    ApprovalDate = DateTime.Now,
                    AadharNumber = application.AadharNumber,
                    PAN = application.PAN,
                    CustomerImageURL = application.CustomerImageURL,
                    IsActive = true,
                    CreatedBy = approvedBy,
                    CreatedDate = DateTime.Now
                };

                _context.Customers.Add(customer);

                await _context.SaveChangesAsync();

                var maxAccountNumber = await _context.Accounts
                    .IgnoreQueryFilters()
                    .MaxAsync(a => (long?)Convert.ToInt64(a.AccountNumber)) ?? 88855999;

                var newAccountNumber = (maxAccountNumber + 1).ToString();

                var account = new Account
                {
                    AccountNumber = newAccountNumber,
                    CustomerID = customer.CustomerID,
                    AccountTypeID = application.AccountTypeID,
                    Balance = 0,
                    OpenDate = DateTime.Now,
                    Status = "Active",
                    IsActive = true,
                    CreatedBy = approvedBy,
                    CreatedDate = DateTime.Now
                };

                _context.Accounts.Add(account);


                application.Status = ApplicationStatus.Approved;
                application.ApprovedByUserID = approvedBy;
                application.ApprovalDate = DateTime.Now;
                application.ModifiedBy = approvedBy;
                application.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                response.Response = new UserResponse
                {
                    Id = user.Id,
                    UserName = username,
                    FullName = user.FullName,
                    Roles = new List<string> { "Customer" },
                    TemporaryPassword = defaultPassword
                };

            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                response.Errors.Add(new Errors { ErrorCode = "401", ErrorMessage = ex.Message });
            }
            return response;
        }



        private string GenerateRandomPassword()
        {
            var chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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

