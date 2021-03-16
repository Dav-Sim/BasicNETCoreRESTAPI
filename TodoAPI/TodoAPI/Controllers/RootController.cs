using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Models;

namespace TodoAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        public const string GetRootRoute = "GetRoot";
        [HttpGet(Name = GetRootRoute)]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                Url.Link(GetRootRoute, new { }),
                "self",
                "GET"));

            links.Add(new LinkDto(
                Url.Link(TasksController.GetTasksRoute, new { }),
                "tasks",
                "GET"));

            links.Add(new LinkDto(
                Url.Link(TasksController.PostTaskRoute, new { }),
                "create_task",
                "POST"));

            return Ok(links);
        }
    }
}
