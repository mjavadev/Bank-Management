using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Entity.Dto
{
    public class AccountDto
    {
        public int AccountID { get; set; }
        public string AccountNumber { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerName { get; set; }

        [Required]
        public int AccountTypeID { get; set; }

        public string? AccountTypeName { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }
}
