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
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<Result<UserResponse>> Authenticate(UserRequest request)
        {
            Result<UserResponse> response = new();

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || user.IsDeleted)
            {
                response.Errors.Add(new Errors { ErrorCode = "101", ErrorMessage = "Invalid Credential" });
                return response;
            }

            if (!user.IsActive)
            {
                response.Errors.Add(new Errors { ErrorCode = "102", ErrorMessage = "Account is inactive. Please contact administrator." });
                return response;
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Customer"))
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.ApplicationUserID == user.Id && !c.IsDeleted);

                if (customer == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "103", ErrorMessage = "Customer record not found" });
                    return response;
                }

                if (!customer.IsActive)
                {
                    response.Errors.Add(new Errors { ErrorCode = "104", ErrorMessage = "Customer account is inactive" });
                    return response;
                }
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (result)
            {
                response.Response = new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                    MustChangePassword = user.MustChangePassword
                };
            }
            else
            {
                response.Errors.Add(new Errors { ErrorCode = "101", ErrorMessage = "Invalid Credential" });
            }

            return response;
        }

        public async Task<bool> IsAValidUser(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.IsDeleted || !user.IsActive)
                return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Result<UserResponse>> Register(UserRequest request, string role)
        {
            Result<UserResponse> response = new();

            ApplicationUser user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.UserName,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                var roles = await _userManager.GetRolesAsync(user);
                response.Response = new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = roles.ToList()
                };
            }
            else
            {
                foreach (var err in result.Errors)
                {
                    response.Errors.Add(new Errors { ErrorCode = "102", ErrorMessage = err.Description });
                }
            }

            return response;
        }

        public async Task<Result<bool>> ChangePassword(string userId, string currentPassword, string newPassword)
        {
            Result<bool> response = new();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.Errors.Add(new Errors { ErrorCode = "103", ErrorMessage = "User not found" });
                return response;
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);
                response.Response = true;
            }
            else
            {
                foreach (var err in result.Errors)
                {
                    response.Errors.Add(new Errors { ErrorCode = "104", ErrorMessage = err.Description });
                }
            }

            return response;
        }
    }

}



