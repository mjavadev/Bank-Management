using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Entity.Dto
{
    public class EmployeeDto
    {
        public int EmployeeID { get; set; }
        public string ApplicationUserID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        public string? Password { get; set; }

        [Required]
        public string StaffCode { get; set; }

        [Required]
        public string JobTitle { get; set; }

        [Required]
        public DateTime DateHired { get; set; }
    }
}
