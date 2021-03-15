using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TodoAPI.Helpers
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //this binder works only with array
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            //get value
            var value = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName).ToString();

            //if value is null or whitespace return null
            if (string.IsNullOrWhiteSpace(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            //value není null a typ je pole
            //get enumerable type a converter
            var eleType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            var converter = TypeDescriptor.GetConverter(eleType);

            //convert each item
            var values = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => converter.ConvertFromString(a.Trim()))
                .ToArray();

            //create array of that type and set movel value
            var typeValues = Array.CreateInstance(eleType, values.Length);
            values.CopyTo(typeValues, 0);
            bindingContext.Model = typeValues;

            //return successful result, passing in Model
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
