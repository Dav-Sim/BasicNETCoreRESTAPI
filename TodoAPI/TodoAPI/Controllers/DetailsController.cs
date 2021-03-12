using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId}/details")]
    public class DetailsController : ControllerBase
    {
        private const string GetDetailRoute = "GetDetail";
        private readonly TasksRepository _Repo;
        private readonly IMapper _Mapper;

        public DetailsController(TasksRepository repo, IMapper mapper)
        {
            _Repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}
