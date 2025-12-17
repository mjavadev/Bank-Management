using BankApp.Entity.Enums;
using BankApp.Entity.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Entity.Dto
{
    public class ApplicationDto
    {
        public int ApplicationID { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [DateNotInFutureAttribute(ErrorMessage = "Date of birth cannot be in the future")]
        [MinimumAgeAttribute(18, ErrorMessage = "Applicant must be at least 18 years old")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? Occupation { get; set; }

        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number")]
        public required string MobileNumber { get; set; }

        [Required]
        [MaxLength(12)]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhar must be 12 digits")]
        public required string AadharNumber { get; set; }

        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format")]
        public required string PAN { get; set; }

        public string? CustomerImageURL { get; set; }

        [Required]
        public int AccountTypeID { get; set; }

        public ApplicationStatus Status { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? RejectionReason { get; set; }
    }
}
