using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Services.SortingServices
{
    /// <summary>
    /// this class represents destination entity properties for single property of DTO
    /// it is collection, because property of DTO could be concatenated multiple properties of entity
    /// </summary>
    public class PropertyMappingValue
    {
        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationProperties = destinationProperties ?? throw new ArgumentNullException(nameof(destinationProperties));
            Revert = revert;
        }

        /// <summary>
        /// collection of destination entity property names
        /// </summary>
        public IEnumerable<string> DestinationProperties { get; private set; }

        /// <summary>
        /// revert is used for example if DTO property is Age but Entity property is DateOfBirth
        /// </summary>
        public bool Revert { get; set; }
    }
}
