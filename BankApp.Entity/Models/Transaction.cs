using BankApp.Entity.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BankApp.Entity.Enums;


namespace BankApp.Entity.Models
{
    [Table("tblTransactions")]
    public class Transaction : BaseEntity
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public int AccountID { get; set; }

        [ForeignKey("AccountID")]
        public Account? Account { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [MaxLength(200)]
        public string? Description { get; set; }

        public int? RecipientAccountID { get; set; }

        [ForeignKey("RecipientAccountID")]
        public Account? RecipientAccount { get; set; }

        public string? ProcessedByUserID { get; set; }

        [ForeignKey("ProcessedByUserID")]
        public ApplicationUser? ProcessedByUser { get; set; }

        public Enums.TransactionStatus Status { get; set; } = Enums.TransactionStatus.Pending;

        public DateTime? ApprovalDate { get; set; }

    }
}
