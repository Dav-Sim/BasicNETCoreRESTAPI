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
                return ValidateModel(new List<Models.TaskForManipulationDTO>() { model });
            }
            else if (validationContext.ObjectInstance is IEnumerable<Models.TaskForManipulationDTO> array)
            {
                return ValidateModel(array);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"input for validation is not {nameof(Models.TaskForManipulationDTO)}");
            }
        }

        private ValidationResult ValidateModel(IEnumerable<Models.TaskForManipulationDTO> models)
        {
            foreach (var model in models)
            {
                if (model.Name == model.Description)
                {
                    return new ValidationResult(
                        ErrorMessage ?? DefaultMessage,
                        new string[] { nameof(Models.TaskForManipulationDTO) });
                }
            }

            return ValidationResult.Success;
        }
    }
}
