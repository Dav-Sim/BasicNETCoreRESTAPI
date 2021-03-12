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
        public void DeleteDetail(Entities.Detail detail);
        public Entities.Detail UpdateDetail(Guid taskId, Guid detailId, Entities.Detail detail);
        public Entities.Detail CreateDetail(Guid taskId, Entities.Detail detail);
        public Entities.Detail CreateDetailwithId(Guid taskId, Guid detailId, Entities.Detail detail);
        public IEnumerable<Entities.Detail> GetDetails(Guid taskId);
        public Entities.Detail GetDetail(Guid taskId, Guid detailId);
    }
}
