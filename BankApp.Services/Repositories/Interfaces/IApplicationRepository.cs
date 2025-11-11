using BankApp.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Services.Repositories.Interfaces
{
    public interface IApplicationRepository
    {
        Task<Result<List<ApplicationDto>>> GetAllApplications();
        Task<Result<List<ApplicationDto>>> GetPendingApplications();
        Task<Result<ApplicationDto>> GetApplicationById(int id);
        Task<Result<ApplicationDto>> CreateApplication(ApplicationDto application);
        Task<Result<bool>> UpdateApplication(int id, ApplicationDto applicationDto, string modifiedBy);
        Task<Result<UserResponse>> ApproveApplication(int applicationId, string approvedBy);
        Task<Result<bool>> RejectApplication(int applicationId, string rejectedBy, string reason);
    }
}
