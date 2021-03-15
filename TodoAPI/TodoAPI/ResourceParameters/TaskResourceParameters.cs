using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.ResourceParameters
{
    public class TaskResourceParameters
    {
        private const int maxPageSize = 20;
        private int pageSize = 5;

        [FromQuery(Name = "name")]
        public string NameExact { get; set; }
        [FromQuery(Name = "search")]
        public string Search { get; set; }
        [FromQuery(Name = "priority")]
        public int? Priority { get; set; }
        [FromQuery(Name = "priority.gt")]
        public int? PriorityGT { get; set; }
        [FromQuery(Name = "priority.lt")]
        public int? PriorityLT { get; set; }

        /// <summary>
        /// page number for pagination, default is 1
        /// </summary>
        public int PageNumber { get; set; } = 1;
        /// <summary>
        /// page size for pagination, default is 5 maximum 20
        /// </summary>
        public int PageSize { 
            get => pageSize; 
            set => pageSize = value > maxPageSize ? maxPageSize : value; 
        }
        /// <summary>
        /// Order by query string
        /// </summary>
        public string orderBy { get; set; } = "Name";
        /// <summary>
        /// fields for data shaping
        /// </summary>
        public string Fields { get; set; }
    }
}
