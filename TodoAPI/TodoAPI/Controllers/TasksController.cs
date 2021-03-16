using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Helpers;
using TodoAPI.ResourceParameters;
using TodoAPI.Services;
using TodoAPI.Services.SortingServices;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        public const string GetTaskRoute = "GetTask";
        public const string DeleteTaskRoute = "DeleteTask";
        public const string PostTaskRoute = "PostTask";
        public const string PutTaskRoute = "PutTask";
        public const string PatchTaskRoute = "PatchTask";
        public const string GetTasksRoute = "GetTasks";
        private readonly ITasksRepository _Repo;
        private readonly IMapper _Mapper;
        private readonly IPropertyMappingService _PropertyMappingService;
        private readonly IPropertyCheckerService _PropertyChecker;

        public TasksController(ITasksRepository repository, IMapper mapper, IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyChecker)
        {
            _Repo = repository ?? throw new ArgumentNullException(nameof(repository));
            _Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _PropertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _PropertyChecker = propertyChecker ?? throw new ArgumentNullException(nameof(propertyChecker));
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
        [HttpGet(Name = GetTasksRoute)]
        [HttpHead]
        public IActionResult GetTasks(
            [FromQuery] TaskResourceParameters parameters)
        {
            //check property mapping DTO to Entity (for sorting)
            if (!_PropertyMappingService
                .ValidMappingExistsFor<Models.TaskDTO, Entities.Task>(parameters.orderBy))
            {
                return BadRequest();
            }

            //check data shaping fields request
            if (!_PropertyChecker.TypeHasProperties<Models.TaskDTO>(parameters.Fields))
            {
                return BadRequest();
            }

            //get tasks
            var tasks = _Repo.GetAll(parameters);

            //add paging informations
            var paginationMetaData = new
            {
                totalCount = tasks.TotalCount,
                pageSize = tasks.PageSize,
                currentPage = tasks.CurrentPage,
                totalPages = tasks.TotalPages,
            };
            //paging metadata add to custom header named "X-Pagination"
            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetaData));

            //map to DTOs
            var taskDtos = _Mapper.Map<IEnumerable<Models.TaskDTO>>(tasks)
                .ShapeData(parameters.Fields);

            //create and add links to each task
            var tasksWithLinks = taskDtos.Select(t => {
                var taskAsDict = t as IDictionary<string, object>;
                var taskLinks = CreateLinksForTask((Guid)taskAsDict["Id"]);
                taskAsDict.Add("links", taskLinks);
                return taskAsDict;
            });
            //create links for collection
            var links = CreateLinksForTasks(parameters,
                tasks.HasNext, 
                tasks.HasPrevious);
            //create object with links and collection and return it
            var linkedCollection = new
            {
                value = tasksWithLinks,
                links = links
            };
            
            //return 200 with object in body
            return Ok(linkedCollection);
        }

        /// <summary>
        /// GET one task
        /// </summary>
        /// <returns>200/404</returns>
        [HttpGet("{id}", Name = GetTaskRoute)]
        [HttpHead("{id}")]
        public IActionResult GetTask(Guid id, string fields)
        {
            //check data shaping fields request
            if (!_PropertyChecker.TypeHasProperties<Models.TaskDTO>(fields))
            {
                return BadRequest();
            }

            //get task
            var task = _Repo.GetOne(id);

            //if null return 404
            if (task == null)
            {
                return NotFound();
            }

            //map to DTO
            var taskDto = _Mapper.Map<Models.TaskDTO>(task);

            //shape data
            var shapedDto = taskDto.ShapeData(fields) as IDictionary<string, object>;

            //create hateoas links
            var links = CreateLinksForTask(id, fields);
            shapedDto.Add("links", links);

            //return 200 with DTO in body
            return Ok(shapedDto);
        }

        /// <summary>
        /// POST create task
        /// </summary>
        /// <returns>201 with Link in header</returns>
        [HttpPost(Name = PostTaskRoute)]
        public ActionResult<Models.TaskDTO> CreateTask(Models.TaskForCreatingDTO task)
        {
            //if task is null or validation failed unprocessable entity 422 is returned

            //map from DTO to ENTITY
            var taskEntity = _Mapper.Map<Entities.Task>(task);

            //create task
            var createdTask = _Repo.Create(taskEntity);

            //map created ENTITY to DTO
            var result = _Mapper.Map<Models.TaskDTO>(createdTask);

            //hateoas links
            var links = CreateLinksForTask(result.Id);
            var linkedResourceToReturn = result.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            //return 201 with Link to task
            return CreatedAtRoute(
                GetTaskRoute,
                new { id = result.Id },
                linkedResourceToReturn);
        }

        /// <summary>
        /// PUT upade task
        /// </summary>
        /// <returns>204/404</returns>
        [HttpPut("{id}", Name = PutTaskRoute)]
        public IActionResult UpdateTask(Guid id, Models.TaskForUpdatingDTO updatedDto)
        {
            //get task
            var task = _Repo.GetOne(id);

            //if null return 404
            if (task == null)
            {
                return NotFound();

                //here should be UPSERT instead of 404

                //here is UPSERT code if we want to
                /*
                UpsertTask(id, updatedDto);
                */
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
        [HttpPatch("{id}", Name = PatchTaskRoute)]
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

                //here is UPSERT code if we want to
                /*
                var taskDto = new Models.TaskForUpdatingDTO();
                patch.ApplyTo(taskDto, ModelState);
                if (!TryValidateModel(taskDto))
                {
                    return ValidationProblem(ModelState);
                }
                UpsertTask(id, taskDto);
                */
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
        [HttpDelete("{id}", Name = DeleteTaskRoute)]
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
        /// UPSERT code for creating task by PUT or PATCH
        /// </summary>
        /// <returns>201 created at route</returns>
        private ActionResult UpsertTask(Guid taskId, Models.TaskForUpdatingDTO taskDto)
        {
            var taskToAdd = _Mapper.Map<Entities.Task>(taskDto);
            taskToAdd.Id = taskId;

            Entities.Task addedTask = _Repo.CreateTaskWithSpecifiedId(taskId, taskToAdd);

            var taskToReturn = _Mapper.Map<Models.TaskDTO>(addedTask);
            return CreatedAtRoute(
                GetTaskRoute,
                new { taskId = taskToReturn.Id },
                taskToReturn
                );
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

        /// <summary>
        /// creates link to next or previous page
        /// </summary>
        private string CreateTasksResourceUri(
            TaskResourceParameters parameters,
            ResourceUriType type)
        {
            var newParameters = new Dictionary<string, object>();
            newParameters.Add("fields", parameters.Fields);
            newParameters.Add("orderBy", parameters.orderBy);
            newParameters.Add("pageSize", parameters.PageSize);
            newParameters.Add("name", parameters.NameExact);
            newParameters.Add("search", parameters.Search);
            newParameters.Add("priority", parameters.Priority);
            newParameters.Add("priority.gt", parameters.PriorityGT);
            newParameters.Add("priority.lt", parameters.PriorityLT);
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    newParameters.Add("pageNumber", parameters.PageNumber - 1);
                    return Url.Link(
                        GetTasksRoute,
                        newParameters);
                case ResourceUriType.NextPage:
                    newParameters.Add("pageNumber", parameters.PageNumber + 1);
                    return Url.Link(
                        GetTasksRoute,
                        newParameters);
                case ResourceUriType.Current:
                default:
                    newParameters.Add("pageNumber", parameters.PageNumber);
                    return Url.Link(
                        GetTasksRoute,
                        newParameters);
            }
        }

        /// <summary>
        /// create hateoas links collection for task
        /// </summary>
        private IEnumerable<Models.LinkDto> CreateLinksForTask(Guid taskId, string fields = null)
        {
            var links = new List<Models.LinkDto>();
            object getRouteValues;
            if (string.IsNullOrWhiteSpace(fields))
            {
                getRouteValues = new { id = taskId };
            }
            else
            {
                getRouteValues = new { id = taskId, fields };
            }

            links.Add(new Models.LinkDto(
                Url.Link(GetTaskRoute, getRouteValues),
                "self",
                "GET"));
            links.Add(new Models.LinkDto(
                Url.Link(DeleteTaskRoute, new { id = taskId }),
                "delete",
                "DELETE"));


            return links;
        }

        /// <summary>
        /// create hateoas links collection for task
        /// </summary>
        private IEnumerable<Models.LinkDto> CreateLinksForTasks(
            TaskResourceParameters parameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<Models.LinkDto>();

            //self
            links.Add(new Models.LinkDto(
                CreateTasksResourceUri(parameters, ResourceUriType.Current),
                "self", "GET"));

            //in our implementation collection cannot be deleted nor updated, so self is only one link 

            //next and previous page links
            if (hasNext)
            {
                links.Add(new Models.LinkDto(
                CreateTasksResourceUri(parameters, ResourceUriType.NextPage),
                "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new Models.LinkDto(
                CreateTasksResourceUri(parameters, ResourceUriType.PreviousPage),
                "previousPage", "GET"));
            }

            return links;
        }
    }
}
