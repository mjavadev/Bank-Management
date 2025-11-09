using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Entity.Models
{
    public abstract class BaseEntity
    {
        public bool IsDeleted { get; set; } = false;

        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(450)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [MaxLength(450)]
        public string? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
