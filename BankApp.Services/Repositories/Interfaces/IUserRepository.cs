using BankApp.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Services.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<Result<UserResponse>> Authenticate(UserRequest request);
        Task<Result<UserResponse>> Register(UserRequest request, string role);
        Task<bool> IsAValidUser(string username, string password);
        Task<Result<bool>> ChangePassword(string userId, string currentPassword, string newPassword);
    }
}
