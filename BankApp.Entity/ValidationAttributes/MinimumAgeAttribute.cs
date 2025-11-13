using System;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Entity.ValidationAttributes
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }
                if (age < _minimumAge)
                {
                    return new ValidationResult(ErrorMessage ?? $"Must be at least {_minimumAge} years old");
                }
            }

            return ValidationResult.Success;
        }
    }
}
