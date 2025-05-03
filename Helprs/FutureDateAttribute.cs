using System.ComponentModel.DataAnnotations;

namespace CPMP.Helprs
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly dueDate)
            {
                if (dueDate <= DateOnly.FromDateTime(DateTime.Today))
                {
                    return new ValidationResult(ErrorMessage??"Due date must be after today");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
