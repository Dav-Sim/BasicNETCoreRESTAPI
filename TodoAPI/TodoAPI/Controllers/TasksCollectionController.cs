using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Helpers;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/taskscollection")]
    public class TasksCollectionController : ControllerBase
    {
        public const string GetCollectionRoute = "GetCollection";
        private ITasksRepository _Repo;
        private IMapper _Mapper;
        public TasksCollectionController(ITasksRepository repo, IMapper mapper)
        {
            _Repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// POST - create collection of tasks
        /// </summary>
        [HttpPost]
        public ActionResult<IEnumerable<TaskDTO>> CreateTodoCollection(
            IEnumerable<TaskForCreatingDTO> tasksCollection
            )
        {
            var tasks = _Mapper.Map<IEnumerable<Entities.Task>>(tasksCollection);

            List<Entities.Task> createdTasks = new List<Entities.Task>();
            foreach (var task in tasks)
            {
                var resultTask = _Repo.Create(task);
                createdTasks.Add(resultTask);
            }

            var idsAsString = string.Join(",", createdTasks.Select(a => a.Id));
            var resultCollectionOfDtos = _Mapper.Map<IEnumerable<TaskDTO>>(createdTasks);

            return CreatedAtRoute(
                GetCollectionRoute,
                new { ids = idsAsString },
                resultCollectionOfDtos
                );
        }

        /// <summary>
        /// GET with composite key from route (id1,id2,id3...) parsed with custom binder ArrayModelBinder
        /// </summary>
        [HttpGet("({ids})", Name = GetCollectionRoute)]
        public ActionResult<IEnumerable<TaskDTO>> GetToDoCollection(
            [FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids
            )
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var tasks = _Repo.GetAll(ids);

            if (ids.Count() != tasks.Count())
            {
                return NotFound();
            }

            var todosDto = _Mapper.Map<IEnumerable<TaskDTO>>(tasks);
            return Ok(todosDto);
        }
    }
}
