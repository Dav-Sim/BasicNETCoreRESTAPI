using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Services.SortingServices
{
    /// <summary>
    /// mapping between DTO property and Entity property/ies
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        /// <summary>
        /// dictionary where key is DTO property name, and value is class wrapping destination entity property names 
        /// </summary>
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary ??
                throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}
