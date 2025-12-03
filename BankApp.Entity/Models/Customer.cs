using BankApp.Entity.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Entity.Models
{
    [Table("tblCustomers")]
    public class Customer:BaseEntity
    {
        [Key]
        public int CustomerID { get; set; }

        [Required]
        public string? ApplicationUserID { get; set; }

        [ForeignKey("ApplicationUserID")]
        public ApplicationUser? ApplicationUser { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? Occupation { get; set; }

        [Required]
        [MaxLength(10)]
        public string? MobileNumber { get; set; }

        public string? ApprovedByUserID { get; set; }

        [ForeignKey("ApprovedByUserID")]
        public ApplicationUser? ApprovedByUser { get; set; }

        public DateTime? ApprovalDate { get; set; }

        [MaxLength(12)]
        public string? AadharNumber { get; set; }

        [MaxLength(10)]
        public string? PAN { get; set; }

        public string? CustomerImageURL { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<Account>? Accounts { get; set; }
    }
}
