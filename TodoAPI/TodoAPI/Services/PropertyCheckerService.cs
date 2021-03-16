using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Services
{
    public class PropertyCheckerService : IPropertyCheckerService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var split = fields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var field in split)
            {
                var trimmed = field.Trim();

                var propInfo = typeof(T)
                    .GetProperty(trimmed,
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (propInfo == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
