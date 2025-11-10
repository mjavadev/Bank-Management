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
                //var account = await _context.Accounts.FindAsync(transactionDto.AccountID);
                var account = await _context.Accounts
                    .Include(a => a.Customer)
                    .FirstOrDefaultAsync(a => a.AccountID == transactionDto.AccountID);

                if (account == null || account.IsDeleted)
                {
                    response.Errors.Add(new Errors { ErrorCode = "503", ErrorMessage = "Account not found" });
                    return response;
                }

                // VALIDATION: Check if account is active
                if (!account.IsActive)
                {
                    response.Errors.Add(new Errors { ErrorCode = "507", ErrorMessage = "Account is inactive" });
                    return response;
                }

                // VALIDATION: Check if customer is active
                if (!account.Customer.IsActive)
                {
                    response.Errors.Add(new Errors { ErrorCode = "508", ErrorMessage = "Customer is inactive" });
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

                // VALIDATION: If transfer, check recipient account is active
                if (transactionDto.TransactionType == TransactionType.Transfer)
                {
                    var recipientAccount = await _context.Accounts
                        .Include(a => a.Customer)
                        .FirstOrDefaultAsync(a => a.AccountID == transactionDto.RecipientAccountID);

                    if (recipientAccount == null || recipientAccount.IsDeleted)
                    {
                        response.Errors.Add(new Errors { ErrorCode = "509", ErrorMessage = "Recipient account not found" });
                        return response;
                    }

                    if (!recipientAccount.IsActive)
                    {
                        response.Errors.Add(new Errors { ErrorCode = "510", ErrorMessage = "Recipient account is inactive" });
                        return response;
                    }

                    if (!recipientAccount.Customer.IsActive)
                    {
                        response.Errors.Add(new Errors { ErrorCode = "511", ErrorMessage = "Recipient customer is inactive" });
                        return response;
                    }
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
                //var transaction = await _context.Transactions
                //    .Include(t => t.Account)
                //    .Include(t => t.RecipientAccount)
                //    .FirstOrDefaultAsync(t => t.TransactionID == transactionId);

                var transaction = await _context.Transactions
                    .Include(t => t.Account)
                        .ThenInclude(a => a.Customer)
                    .Include(t => t.RecipientAccount)
                        .ThenInclude(a => a.Customer)
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

                // VALIDATION: Check account is still active
                if (!transaction.Account.IsActive || !transaction.Account.Customer.IsActive)
                {
                    response.Errors.Add(new Errors { ErrorCode = "512", ErrorMessage = "Source account or customer is inactive" });
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

                        // VALIDATION: Check recipient is still active
                        if (!transaction.RecipientAccount.IsActive || !transaction.RecipientAccount.Customer.IsActive)
                        {
                            response.Errors.Add(new Errors { ErrorCode = "513", ErrorMessage = "Recipient account or customer is inactive" });
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

