using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.ResourceParameters;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private const string GetTaskRoute = "GetTask";
        private readonly TasksRepository _Repo;
        private readonly IMapper _Mapper;

        public TasksController(TasksRepository repository, IMapper mapper)
        {
            _Repo = repository;
            _Mapper = mapper;
        }

        /// <summary>
        /// OPTIONS
        /// </summary>
        /// <returns>200</returns>
        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET,HEAD,OPTIONS,POST");
            return Ok();
        }

        /// <summary>
        /// GET optionally with query filter
        /// </summary>
        /// <returns>200</returns>
        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<Models.TaskDTO>> GetTasks(
            [FromQuery] TaskResourceParameters parameters)
        {

        }

        /// <summary>
        /// GET one task
        /// </summary>
        /// <returns>200/404</returns>
        [HttpGet("{id}", Name = GetTaskRoute)]
        [HttpHead("{id}")]
        public ActionResult<Models.TaskDTO> GetTask(Guid id)
        {

        }

        /// <summary>
        /// POST create task
        /// </summary>
        /// <returns>201 with Link in header</returns>
        [HttpPost]
        public ActionResult<Models.TaskDTO> CreateTask(Models.TaskForCreatingDTO task)
        {

        }

        /// <summary>
        /// PUT upade task
        /// </summary>
        /// <returns>204/404</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateTask(Guid id, Models.TaskForUpdatingDTO task)
        {

        }

        /// <summary>
        /// PATCH partial update task
        /// </summary>
        /// <returns>204/404</returns>
        [HttpPatch("{id}")]
        public IActionResult PatchTask(Guid id,
            JsonPatchDocument<Models.TaskForUpdatingDTO> patch)
        {

        }

        /// <summary>
        /// DELETE delete task
        /// </summary>
        /// <returns>204/404</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(Guid id)
        {

        }
    }
}
