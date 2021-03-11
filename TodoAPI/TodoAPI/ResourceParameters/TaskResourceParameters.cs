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
        public string Priority { get; set; }
        [FromQuery(Name = "priority.gt")]
        public string PriorityGT { get; set; }
        [FromQuery(Name = "priority.lt")]
        public string PriorityLT { get; set; }
    }
}
