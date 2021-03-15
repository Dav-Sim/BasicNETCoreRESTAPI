using System;
using System.Collections.Generic;
using TodoAPI.Helpers;

namespace TodoAPI.Services
{
    public interface ITasksRepository
    {
        public void Delete(Entities.Task task);
        public Entities.Task Update(Guid id, Entities.Task task);
        public Entities.Task Create(Entities.Task task);
        public PagedList<Entities.Task> GetAll(ResourceParameters.TaskResourceParameters query);
        public IEnumerable<Entities.Task> GetAll(IEnumerable<Guid> ids);
        public Entities.Task GetOne(Guid id);
        public bool TaskExists(Guid id);
        public Entities.Task CreateTaskWithSpecifiedId(Guid taskId, Entities.Task task);
    }
}
