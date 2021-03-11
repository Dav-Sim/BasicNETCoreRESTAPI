using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Services
{
    //Fake repo
    public class TasksRepository
    {
        private List<Entities.Task> _List = new List<Entities.Task>()
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
                Priority = 0,
                Status = Entities.Status.NotStarted,
                Updated = DateTime.Now
            }
        };

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
        public IEnumerable<Entities.Task> GetAll(ResourceParameters.TaskResourceParameters query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            //if no query return all
            if (query.IsEmpty)
            {
                return GetAll();
            }

            //evaluate query and return
            IEnumerable<Entities.Task> collection = _List;
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

            return collection;
        }
        public IEnumerable<Entities.Task> GetAll()
        {
            return _List;
        }
        public Entities.Task GetOne(Guid id)
        {
            return _List.FirstOrDefault(a => a.Id == id);
        }

    }
}
