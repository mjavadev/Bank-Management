using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Entity.Models
{
    [Table("tblAccountTypes")]

    public class AccountType
    {
        [Key]
        public int AccountTypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public decimal InterestRate { get; set; } = 0;

        public bool IsDeleted { get; set; } = false;

        public ICollection<Account> Accounts { get; set; }
    }
}
