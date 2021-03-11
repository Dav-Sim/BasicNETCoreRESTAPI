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
                Priority = 0,
                Status = Entities.Status.NotStarted,
                Updated = DateTime.Now
            },
            new Entities.Task()
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Task2",
                Description = "Desc for Task2",
                Priority = 0,
                Status = Entities.Status.NotStarted,
                Updated = DateTime.Now
            },
            new Entities.Task()
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Task3",
                Description = "Desc for Task3",
                Priority = 0,
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
        public Entities.Task Update(Entities.Task task)
        {
            var i = _List.IndexOf(task);
            _List[i] = task;
            return _List[i];
        }
        public Entities.Task Create(Entities.Task task)
        {
            task.Id = Guid.NewGuid();
            _List.Add(task);
            return task;
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
