using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Models;

namespace TodoAPI.Services.SortingServices
{
    /// <summary>
    /// service for mapping between properties of two different types
    /// </summary>
    public class PropertyMappingService : IPropertyMappingService
    {
        /// <summary>
        /// mappings between DTO props and Entity props
        /// </summary>
        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            //add mapping from TaskDTO properties to Entities.Task properties
            _propertyMappings.Add(new PropertyMapping<TaskDTO, Entities.Task>(
                    new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"Id", new PropertyMappingValue(new List<string>{ "Id" }, false) },
                        {"Name", new PropertyMappingValue(new List<string>{ "Name" }) },
                        {"Description", new PropertyMappingValue(new List<string>{ "Description" }) },
                        {"Priority", new PropertyMappingValue(new List<string>{ "Priority" }) }
                    }
                ));
        }

        /// <summary>
        /// checks if mapping between properties of source type and destination type matches specified fields
        /// </summary>
        /// <returns></returns>
        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            //split fileds
            var splittedFields = fields.ToLower().Split(",");
            //loop throught fileds clauses
            foreach (var field in splittedFields)
            {
                var trimmed = field.Trim();

                var indexOfSpace = trimmed.IndexOf(" ");
                var propName = indexOfSpace == -1 ?
                    trimmed :
                    trimmed.Remove(indexOfSpace);

                //find matching property
                if (!propertyMapping.ContainsKey(propName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// get mapping between properties of source type and destination type
        /// </summary>
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            //get matching mappings
            var matchingMappings = _propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMappings.Count() == 1)
            {
                return matchingMappings.First().MappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}>");
        }
    }
}
