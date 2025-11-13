using BankApp.Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Services.Repositories.Interfaces
{
    public interface IAccountTypeRepository
    {
        Task<List<AccountTypeDto>> GetAllAccountTypesAsync();
    }
}
