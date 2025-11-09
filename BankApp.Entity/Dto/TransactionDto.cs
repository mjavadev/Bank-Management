using BankApp.Entity.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Entity.Dto
{
    public class TransactionDto
    {
        public int TransactionID { get; set; }

        [Required]
        public int AccountID { get; set; }

        public string? AccountNumber { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public int? RecipientAccountID { get; set; }
        public string? RecipientAccountNumber { get; set; }
        public TransactionStatus Status { get; set; }
        public string? ProcessedByName { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
