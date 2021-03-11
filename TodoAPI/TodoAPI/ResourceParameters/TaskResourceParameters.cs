using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.ResourceParameters
{
    public class TaskResourceParameters
    {
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

        public bool IsEmpty
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(NameExact) &&
                    string.IsNullOrWhiteSpace(Search) &&
                    (Priority == null) &&
                    (PriorityGT == null) &&
                    (PriorityLT == null);
            }
        }
    }
}
