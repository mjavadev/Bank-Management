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
    [Table("tblEmployees")]
    public class Employee : BaseEntity
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required]
        public required string ApplicationUserID { get; set; }

        [ForeignKey("ApplicationUserID")]
        public ApplicationUser? ApplicationUser { get; set; }

        [Required]
        [MaxLength(20)]
        public required string StaffCode { get; set; }

        [Required]
        [MaxLength(50)]
        public required string JobTitle { get; set; }

        [Required]
        public DateTime DateHired { get; set; }

    }
}
