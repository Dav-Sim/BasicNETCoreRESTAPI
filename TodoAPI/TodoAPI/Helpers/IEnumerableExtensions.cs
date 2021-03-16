using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TodoAPI.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(
            this IEnumerable<TSource> source,
            string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            //create list of expando object
            var result = new List<ExpandoObject>();

            var propertyInfos = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                propertyInfos.AddRange(typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance));
            }
            else
            {
                var split = fields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var field in split)
                {
                    var propName = field.Trim();

                    var propInfo = typeof(TSource)
                        .GetProperty(propName,
                        BindingFlags.IgnoreCase |
                        BindingFlags.Public | BindingFlags.Instance);

                    if (propInfo == null)
                    {
                        throw new Exception($"Property {propName} wasnt found on {typeof(TSource)}");
                    }

                    propertyInfos.Add(propInfo);
                }
            }

            foreach (TSource sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();

                foreach (var propInfo in propertyInfos)
                {
                    var propValue = propInfo.GetValue(sourceObject);

                    if (!dataShapedObject.TryAdd(propInfo.Name, propValue))
                    {
                        throw new Exception($"Cannot add property {propInfo.Name} to data shape object");
                    }
                }

                result.Add(dataShapedObject);
            }

            return result;
        }
    }
}
