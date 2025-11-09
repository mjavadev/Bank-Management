using BankApp.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Services.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Result<List<TransactionDto>>> GetAllTransactions();
        Task<Result<List<TransactionDto>>> GetTransactionsByAccountId(int accountId);
        Task<Result<List<TransactionDto>>> GetPendingTransactions();
        Task<Result<TransactionDto>> GetTransactionById(int id);
        Task<Result<TransactionDto>> CreateTransaction(TransactionDto transaction, string createdBy);
        Task<Result<bool>> ApproveTransaction(int transactionId, string approvedBy);
        Task<Result<bool>> RejectTransaction(int transactionId, string rejectedBy);
    }
}
