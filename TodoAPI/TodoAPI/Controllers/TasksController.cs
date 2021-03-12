using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            _Repo = repository ?? throw new ArgumentNullException(nameof(repository));
            _Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// OPTIONS
        /// </summary>
        /// <returns>200</returns>
        [HttpOptions]
        public IActionResult GetOptions()
        {
            //add Allowed method to headers
            Response.Headers.Add("Allow", "GET,HEAD,OPTIONS,POST");
            
            //return 200
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
            //get tasks
            var tasks = _Repo.GetAll(parameters);

            //map to DTOs
            var taskDtos = _Mapper.Map<IEnumerable<Models.TaskDTO>>(tasks);
            
            //return 200 with DTO in body
            return Ok(taskDtos);
        }

        /// <summary>
        /// GET one task
        /// </summary>
        /// <returns>200/404</returns>
        [HttpGet("{id}", Name = GetTaskRoute)]
        [HttpHead("{id}")]
        public ActionResult<Models.TaskDTO> GetTask(Guid id)
        {
            //get task
            var task = _Repo.GetOne(id);

            //if null return 404
            if (task == null)
            {
                return NotFound();
            }

            //map to DTO
            var taskDto = _Mapper.Map<Models.TaskDTO>(task);

            //return 200 with DTO in body
            return Ok(taskDto);
        }

        /// <summary>
        /// POST create task
        /// </summary>
        /// <returns>201 with Link in header</returns>
        [HttpPost]
        public ActionResult<Models.TaskDTO> CreateTask(Models.TaskForCreatingDTO task)
        {
            //if task is null or validation failed unprocessable entity 422 is returned

            //map from DTO to ENTITY
            var taskEntity = _Mapper.Map<Entities.Task>(task);

            //create task
            var createdTask = _Repo.Create(taskEntity);

            //map created ENTITY to DTO
            var result = _Mapper.Map<Models.TaskDTO>(createdTask);

            //return 201 with Link to task
            return CreatedAtRoute(
                GetTaskRoute,
                new { id = result.Id },
                result);
        }

        /// <summary>
        /// PUT upade task
        /// </summary>
        /// <returns>204/404</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateTask(Guid id, Models.TaskForUpdatingDTO updatedDto)
        {
            //get task
            var task = _Repo.GetOne(id);

            //if null return 404
            if (task == null)
            {
                return NotFound();
            }

            //map from DTO to ENTITY
            var updatedtask = _Mapper.Map<Entities.Task>(updatedDto);

            //update
            var resultTask = _Repo.Update(id, updatedtask);

            //map from ENTITY to DTO
            var resultDto = _Mapper.Map<Models.TaskDTO>(resultTask);

            //return 204
            return NoContent();
        }

        /// <summary>
        /// PATCH partial update task
        /// </summary>
        /// <returns>204/404</returns>
        [HttpPatch("{id}")]
        public IActionResult PatchTask(Guid id,
            JsonPatchDocument<Models.TaskForUpdatingDTO> patch)
        {
            //get task
            var task = _Repo.GetOne(id);

            //if null return 404
            if (task == null)
            {
                return NotFound();

                //here should be UPSERT instead of 404
            }

            //map ENTITY to DTO (model for updating)
            var taskToPatch = _Mapper.Map<Models.TaskForUpdatingDTO>(task);

            //apply patch (if we provide modelstate, then errors goes there)
            patch.ApplyTo(taskToPatch, ModelState);

            //validate patch
            if (!TryValidateModel(taskToPatch))
            {
                return ValidationProblem(ModelState);
            }

            //from here is same as PUT
            //map from DTO to ENTITY
            var updatedtask = _Mapper.Map<Entities.Task>(taskToPatch);

            //update
            var resultTask = _Repo.Update(id, updatedtask);

            //map from ENTITY to DTO
            var resultDto = _Mapper.Map<Models.TaskDTO>(resultTask);

            //return 204
            return NoContent();
        }

        /// <summary>
        /// DELETE delete task
        /// </summary>
        /// <returns>204/404</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(Guid id)
        {
            //get task
            Entities.Task task = _Repo.GetOne(id);
            //if not exits return 404
            if (task == null)
            {
                return NotFound();
            }
            //delete
            _Repo.Delete(task);
            //return 204
            return NoContent();
        }


        /// <summary>
        /// override validation in order to return 422 on validation problem instead of 400
        /// </summary>
        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            //get our ApiBehaviorOptions (set up in startup.cs)
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();

            //return our implementation of InvalidModelStateResponseFactory (returns 422...)
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
