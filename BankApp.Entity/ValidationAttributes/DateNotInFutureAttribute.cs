using System;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Entity.ValidationAttributes
{
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date > DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Date cannot be in the future");
                }
            }
            return ValidationResult.Success;
        }
    }
}
