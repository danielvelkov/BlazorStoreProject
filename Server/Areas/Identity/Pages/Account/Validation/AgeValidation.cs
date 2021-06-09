using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Areas.Identity.Pages.Account.Validation
{
    public class AgeValidation : ValidationAttribute
    {
        public int MinAge { get; }
        public int MaxAge { get; }

        public AgeValidation(int minAge, int maxAge)
        {
            MinAge = minAge;
            MaxAge = maxAge;
        }

        public string TooOldErrorMessage() =>
            $"You are either a mummy or a vampier. Get out!";

        public string TooYoungErrorMessage() =>
            $"What are you doing here? Like you got money for my wares.Little baby";

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            var currentDate = DateTime.Now;
            var dateOfBirth = (DateTime)value;
            if (currentDate.Year - dateOfBirth.Year < MinAge)
            {
                return new ValidationResult(TooYoungErrorMessage());
            }
            else if (currentDate.Year - dateOfBirth.Year > MaxAge)
            {
                return new ValidationResult(TooOldErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}