using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Entity.Models
{
    [Table("tblAccounts")]
    public class Account : BaseEntity
    {
        [Key]
        public int AccountID { get; set; }

        [Required]
        [MaxLength(20)]
        public required string AccountNumber { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public Customer? Customer { get; set; }

        [Required]
        public int AccountTypeID { get; set; }

        [ForeignKey("AccountTypeID")]
        public AccountType? AccountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        public DateTime OpenDate { get; set; } = DateTime.Now;

        [MaxLength(10)]
        public string Status { get; set; } = "Active";

        public bool IsActive { get; set; } = true;

        public ICollection<Transaction>? Transactions { get; set; }

    }
}
