using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Helpers;
using TodoAPI.Services.SortingServices;

namespace TodoAPI.Services
{
    //Fake repo
    public class TasksRepository : ITasksRepository
    {
        private List<Entities.Task> _List = new List<Entities.Task>();
        private readonly IPropertyMappingService _PropertyMapping;

        public TasksRepository(IPropertyMappingService propertyMapping)
        {
            this._PropertyMapping = propertyMapping ?? 
                throw new ArgumentNullException(nameof(propertyMapping));

            _List = new List<Entities.Task>()
            {
                new Entities.Task()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Task1",
                    Description = "Desc for Task1",
                    Priority = 1,
                    Status = Entities.Status.NotStarted,
                    Updated = DateTime.Now
                },
                new Entities.Task()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "Task2",
                    Description = "Desc for Task2",
                    Priority = 100,
                    Status = Entities.Status.NotStarted,
                    Updated = DateTime.Now
                },
                new Entities.Task()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    Name = "Task3",
                    Description = "Desc for Task3",
                    Priority = 3,
                    Status = Entities.Status.NotStarted,
                    Updated = DateTime.Now
                },
                new Entities.Task()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Name = "Task4",
                    Description = "Desc for Task4",
                    Priority = 1,
                    Status = Entities.Status.NotStarted,
                    Updated = DateTime.Now
                },
                new Entities.Task()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                    Name = "Task5",
                    Description = "Desc for Task5",
                    Priority = 3,
                    Status = Entities.Status.NotStarted,
                    Updated = DateTime.Now
                },
                new Entities.Task()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                    Name = "Task6",
                    Description = "Desc for Task6",
                    Priority = 2,
                    Status = Entities.Status.NotStarted,
                    Updated = DateTime.Now
                }
            };       
        }

        public void Delete(Entities.Task task)
        {
            _List.Remove(task);
        }
        public Entities.Task Update(Guid id, Entities.Task task)
        {
            var i = _List.FindIndex(a => a.Id == id);
            task.Id = id;
            _List[i] = task;
            return _List[i];
        }
        public Entities.Task Create(Entities.Task task)
        {
            task.Id = Guid.NewGuid();
            _List.Add(task);
            return task;
        }
        public PagedList<Entities.Task> GetAll(ResourceParameters.TaskResourceParameters query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            //evaluate query and return
            IQueryable<Entities.Task> collection = _List.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower().Trim();
                collection = collection.Where(a => a.Name.ToLower().Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(query.NameExact))
            {
                var exact = query.NameExact;
                collection = collection.Where(a => a.Name == exact);
            }
            if (query.Priority != null)
            {
                collection = collection.Where(a => a.Priority == query.Priority.Value);
            }
            if (query.PriorityGT != null)
            {
                collection = collection.Where(a => a.Priority > query.PriorityGT.Value);
            }
            if (query.PriorityLT != null)
            {
                collection = collection.Where(a => a.Priority < query.PriorityLT.Value);
            }

            //order by
            if (!string.IsNullOrWhiteSpace(query.orderBy))
            {
                //get prop mapping dictionary
                var taskPropertyMappingDictionary =
                    _PropertyMapping.GetPropertyMapping<Models.TaskDTO, Entities.Task>();

                //apply sort
                collection = collection.ApplySort(
                    query.orderBy,
                    taskPropertyMappingDictionary);
            }

            return PagedList<Entities.Task>.Create(
                collection.AsQueryable(),
                query.PageNumber,
                query.PageSize);

            //return collection
            //    .Skip((query.PageNumber - 1) * query.PageSize)
            //    .Take(query.PageSize);
        }
        public Entities.Task GetOne(Guid id)
        {
            return _List.FirstOrDefault(a => a.Id == id);
        }
        public IEnumerable<Entities.Task> GetAll(IEnumerable<Guid> ids)
        {
            return _List.Where(t => ids.Any(id => t.Id == id));
        }
        public bool TaskExists(Guid id)
        {
            return GetOne(id) != null;
        }
        public Entities.Task CreateTaskWithSpecifiedId(Guid taskId, Entities.Task task)
        {
            task.Id = taskId;
            _List.Add(task);
            return task;
        }
    }
    }
}
