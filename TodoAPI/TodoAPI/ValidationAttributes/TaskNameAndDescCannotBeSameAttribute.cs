using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.ValidationAttributes
{
    public class TaskNameAndDescCannotBeSameAttribute : ValidationAttribute
    {
        private const string DefaultMessage = "Name and Description cannot have same value";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is Models.TaskForManipulationDTO model)
            {
                if (model.Name == model.Description)
                {
                    return new ValidationResult(
                        ErrorMessage ?? DefaultMessage,
                        new string[] { nameof(Models.TaskForManipulationDTO) });
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException($"input for validation is not {nameof(Models.TaskForManipulationDTO)}");
            }

            return ValidationResult.Success;
        }
    }
}
