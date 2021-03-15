using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TodoAPI.Services.SortingServices;

namespace TodoAPI.Helpers
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// sort iqueryable by combination of input string (order by clauses delimited with comma) and 
        /// mapping dictionary between property names in clause and properties on type T
        /// </summary>
        /// <param name="source">collection of items</param>
        /// <param name="orderBy">order by clauses string, delimited with comma</param>
        /// <param name="mappingDictionary">dictionary where key is property in sort clause, 
        /// and value is class wrapping corresponding properties on type T</param>
        /// <returns></returns>
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source,
            string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            //no order by clause
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            //order by string split by comma
            var orderSplit = orderBy.ToLower().Split(",");

            //result orderby string
            string orderByString = "";

            //apply each order by clause
            foreach (var orderByClause in orderSplit)
            {
                var orderByClauseTrimmed = orderByClause.Trim();

                //descending
                var desc = orderByClause.EndsWith(" desc");

                int spaceIndex = orderByClauseTrimmed.IndexOf(" ");
                //property name
                var propertyName = spaceIndex == -1 ?
                    orderByClauseTrimmed :
                    orderByClauseTrimmed.Remove(spaceIndex);

                //find matching property
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                //get property mapping value
                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null)
                {
                    throw new ArgumentException(nameof(propertyMappingValue));
                }

                foreach (var destProperty in propertyMappingValue.DestinationProperties)
                {
                    //revert if neccessary
                    if (propertyMappingValue.Revert)
                    {
                        desc = !desc;
                    }

                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ",")
                        + destProperty
                        + (desc ? " descending" : " ascending");
                }
            }

            return source.OrderBy(orderByString);
        }
    }
}
