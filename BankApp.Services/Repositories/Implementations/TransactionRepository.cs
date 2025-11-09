using BankApp.Entity.Dto;
using BankApp.Entity.Enums;
using BankApp.Entity.Models;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.BankingApi.Entity.Models;

namespace BankApp.Services.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<TransactionDto>>> GetAllTransactions()
        {
            Result<List<TransactionDto>> response = new();

            try
            {
                var transactions = await _context.Transactions
                    .Where(t => !t.IsDeleted)
                    .Include(t => t.Account)
                    .Include(t => t.RecipientAccount)
                    .Include(t => t.ProcessedByUser)
                    .Select(t => new TransactionDto
                    {
                        TransactionID = t.TransactionID,
                        AccountID = t.AccountID,
                        AccountNumber = t.Account.AccountNumber,
                        TransactionType = t.TransactionType,
                        Amount = t.Amount,
                        TransactionDate = t.TransactionDate,
                        Description = t.Description,
                        RecipientAccountID = t.RecipientAccountID,
                        RecipientAccountNumber = t.RecipientAccount != null ? t.RecipientAccount.AccountNumber : null,
                        Status = t.Status,
                        ProcessedByName = t.ProcessedByUser != null ? t.ProcessedByUser.FullName : null,
                        ApprovalDate = t.ApprovalDate
                    })
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();

                response.Response = transactions;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "501", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<List<TransactionDto>>> GetTransactionsByAccountId(int accountId)
        {
            Result<List<TransactionDto>> response = new();

            try
            {
                var transactions = await _context.Transactions
                    .Where(t => !t.IsDeleted && (t.AccountID == accountId || t.RecipientAccountID == accountId))
                    .Include(t => t.Account)
                    .Include(t => t.RecipientAccount)
                    .Include(t => t.ProcessedByUser)
                    .Select(t => new TransactionDto
                    {
                        TransactionID = t.TransactionID,
                        AccountID = t.AccountID,
                        AccountNumber = t.Account.AccountNumber,
                        TransactionType = t.TransactionType,
                        Amount = t.Amount,
                        TransactionDate = t.TransactionDate,
                        Description = t.Description,
                        RecipientAccountID = t.RecipientAccountID,
                        RecipientAccountNumber = t.RecipientAccount != null ? t.RecipientAccount.AccountNumber : null,
                        Status = t.Status,
                        ProcessedByName = t.ProcessedByUser != null ? t.ProcessedByUser.FullName : null,
                        ApprovalDate = t.ApprovalDate
                    })
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();

                response.Response = transactions;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "501", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<List<TransactionDto>>> GetPendingTransactions()
        {
            Result<List<TransactionDto>> response = new();

            try
            {
                var transactions = await _context.Transactions
                    .Where(t => !t.IsDeleted && t.Status == TransactionStatus.Pending)
                    .Include(t => t.Account)
                    .ThenInclude(a => a.Customer)
                    .ThenInclude(c => c.ApplicationUser)
                    .Include(t => t.RecipientAccount)
                    .Select(t => new TransactionDto
                    {
                        TransactionID = t.TransactionID,
                        AccountID = t.AccountID,
                        AccountNumber = t.Account.AccountNumber,
                        TransactionType = t.TransactionType,
                        Amount = t.Amount,
                        TransactionDate = t.TransactionDate,
                        Description = t.Description,
                        RecipientAccountID = t.RecipientAccountID,
                        RecipientAccountNumber = t.RecipientAccount != null ? t.RecipientAccount.AccountNumber : null,
                        Status = t.Status
                    })
                    .OrderBy(t => t.TransactionDate)
                    .ToListAsync();

                response.Response = transactions;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "501", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<TransactionDto>> GetTransactionById(int id)
        {
            Result<TransactionDto> response = new();

            try
            {
                var transaction = await _context.Transactions
                    .Where(t => t.TransactionID == id && !t.IsDeleted)
                    .Include(t => t.Account)
                    .Include(t => t.RecipientAccount)
                    .Include(t => t.ProcessedByUser)
                    .Select(t => new TransactionDto
                    {
                        TransactionID = t.TransactionID,
                        AccountID = t.AccountID,
                        AccountNumber = t.Account.AccountNumber,
                        TransactionType = t.TransactionType,
                        Amount = t.Amount,
                        TransactionDate = t.TransactionDate,
                        Description = t.Description,
                        RecipientAccountID = t.RecipientAccountID,
                        RecipientAccountNumber = t.RecipientAccount != null ? t.RecipientAccount.AccountNumber : null,
                        Status = t.Status,
                        ProcessedByName = t.ProcessedByUser != null ? t.ProcessedByUser.FullName : null,
                        ApprovalDate = t.ApprovalDate
                    })
                    .FirstOrDefaultAsync();

                if (transaction == null)
                {
                    response.Errors.Add(new Errors { ErrorCode = "502", ErrorMessage = "Transaction not found" });
                }
                else
                {
                    response.Response = transaction;
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "501", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<TransactionDto>> CreateTransaction(TransactionDto transactionDto, string createdBy)
        {
            Result<TransactionDto> response = new();

            try
            {
                var account = await _context.Accounts.FindAsync(transactionDto.AccountID);
                if (account == null || account.IsDeleted)
                {
                    response.Errors.Add(new Errors { ErrorCode = "503", ErrorMessage = "Account not found" });
                    return response;
                }

                if (transactionDto.TransactionType == TransactionType.Withdrawal ||
                    transactionDto.TransactionType == TransactionType.Transfer)
                {
                    if (account.Balance < transactionDto.Amount)
                    {
                        response.Errors.Add(new Errors { ErrorCode = "504", ErrorMessage = "Insufficient balance" });
                        return response;
                    }
                }

                if (transactionDto.TransactionType == TransactionType.Transfer && !transactionDto.RecipientAccountID.HasValue)
                {
                    response.Errors.Add(new Errors { ErrorCode = "505", ErrorMessage = "Recipient account required for transfer" });
                    return response;
                }

                var transaction = new Transaction
                {
                    AccountID = transactionDto.AccountID,
                    TransactionType = transactionDto.TransactionType,
                    Amount = transactionDto.Amount,
                    Description = transactionDto.Description,
                    RecipientAccountID = transactionDto.RecipientAccountID,
                    Status = TransactionStatus.Pending,
                    TransactionDate = DateTime.Now,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                transactionDto.TransactionID = transaction.TransactionID;
                transactionDto.Status = transaction.Status;
                transactionDto.TransactionDate = transaction.TransactionDate;
                response.Response = transactionDto;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "501", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<bool>> ApproveTransaction(int transactionId, string approvedBy)
        {
            Result<bool> response = new();

            try
            {
                var transaction = await _context.Transactions
                    .Include(t => t.Account)
                    .Include(t => t.RecipientAccount)
                    .FirstOrDefaultAsync(t => t.TransactionID == transactionId);

                if (transaction == null || transaction.IsDeleted)
                {
                    response.Errors.Add(new Errors { ErrorCode = "502", ErrorMessage = "Transaction not found" });
                    return response;
                }

                if (transaction.Status != TransactionStatus.Pending)
                {
                    response.Errors.Add(new Errors { ErrorCode = "506", ErrorMessage = "Transaction already processed" });
                    return response;
                }

                switch (transaction.TransactionType)
                {
                    case TransactionType.Deposit:
                        transaction.Account.Balance += transaction.Amount;
                        break;

                    case TransactionType.Withdrawal:
                        if (transaction.Account.Balance < transaction.Amount)
                        {
                            response.Errors.Add(new Errors { ErrorCode = "504", ErrorMessage = "Insufficient balance" });
                            return response;
                        }
                        transaction.Account.Balance -= transaction.Amount;
                        break;

                    case TransactionType.Transfer:
                        if (transaction.Account.Balance < transaction.Amount)
                        {
                            response.Errors.Add(new Errors { ErrorCode = "504", ErrorMessage = "Insufficient balance" });
                            return response;
                        }
                        transaction.Account.Balance -= transaction.Amount;
                        transaction.RecipientAccount.Balance += transaction.Amount;
                        break;
                }

                transaction.Status = TransactionStatus.Approved;
                transaction.ProcessedByUserID = approvedBy;
                transaction.ApprovalDate = DateTime.Now;
                transaction.ModifiedBy = approvedBy;
                transaction.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "501", ErrorMessage = ex.Message });
            }

            return response;
        }

        public async Task<Result<bool>> RejectTransaction(int transactionId, string rejectedBy)
        {
            Result<bool> response = new();

            try
            {
                var transaction = await _context.Transactions.FindAsync(transactionId);
                if (transaction == null || transaction.IsDeleted)
                {
                    response.Errors.Add(new Errors { ErrorCode = "502", ErrorMessage = "Transaction not found" });
                    return response;
                }

                if (transaction.Status != TransactionStatus.Pending)
                {
                    response.Errors.Add(new Errors { ErrorCode = "506", ErrorMessage = "Transaction already processed" });
                    return response;
                }

                transaction.Status = TransactionStatus.Rejected;
                transaction.ProcessedByUserID = rejectedBy;
                transaction.ApprovalDate = DateTime.Now;
                transaction.ModifiedBy = rejectedBy;
                transaction.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                response.Response = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Errors { ErrorCode = "501", ErrorMessage = ex.Message });
            }

            return response;
        }
    }

}

