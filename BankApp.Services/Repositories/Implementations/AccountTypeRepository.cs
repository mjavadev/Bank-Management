using BankApp.Entity.Dto;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.BankingApi.Entity.Models;

namespace BankApp.Services.Repositories.Implementations
{
    public class AccountTypeRepository : IAccountTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<AccountTypeDto>> GetAllAccountTypesAsync()
        {
            return await _context.AccountTypes
                .Where(at => !at.IsDeleted)
                .Select(at => new AccountTypeDto
                {
                    AccountTypeID = at.AccountTypeID,
                    TypeName = at.TypeName
                })
                .ToListAsync();
        }
    }
}
