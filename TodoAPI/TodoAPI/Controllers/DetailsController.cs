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
using TodoAPI.Models;
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
        /// GET single detail
        /// </summary>
        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<Models.DetailDTO>> GetDetails(Guid taskId)
        {
            if (!_Repo.TaskExists(taskId))
            {
                return NotFound();
            }
            var dtos = _Mapper.Map<IEnumerable<DetailDTO>>(_Repo.GetDetails(taskId));
            return Ok(dtos);
        }

        /// <summary>
        /// GET all details for task
        /// </summary>
        [HttpGet("{detailId}", Name = GetDetailRoute)]
        [HttpHead("{detailId}")]
        public ActionResult<IEnumerable<Models.DetailDTO>> GetDetail(Guid taskId, Guid detailId)
        {
            if (!_Repo.TaskExists(taskId))
            {
                return NotFound();
            }

            Entities.Detail detail = _Repo.GetDetail(taskId, detailId);
            if (detail == null)
            {
                return NotFound();
            }
            var dto = _Mapper.Map<DetailDTO>(detail);
            return Ok(dto);
        }

        /// <summary>
        /// POST detail
        /// </summary>
        [HttpPost]
        public ActionResult<Models.DetailDTO> CreateDetail(Guid taskId, Models.DetailForCreatingDTO detail)
        {
            if (!_Repo.TaskExists(taskId))
            {
                return NotFound();
            }

            var detailEntity = _Mapper.Map<Entities.Detail>(detail);
            Entities.Detail resultEntity = _Repo.CreateDetail(taskId, detailEntity);

            var dtoToReturn = _Mapper.Map<DetailDTO>(resultEntity);

            return CreatedAtRoute(
                GetDetailRoute,
                new { taskId = taskId, detailId = resultEntity.Id },
                dtoToReturn
                );
        }

        /// <summary>
        /// PUT update detail
        /// </summary>
        [HttpPut("{detailId}")]
        public ActionResult UpdateDetail(Guid taskId,
            Guid detailId,
            Models.DetailForUpdatingDTO updateDto)
        {
            if (!_Repo.TaskExists(taskId))
            {
                return NotFound();
            }

            Entities.Detail detailEntity = _Repo.GetDetail(taskId, detailId);
            if (detailEntity == null)
            {
                //return NotFound();

                //here is UPSERT code if we want to - in this case yes
                return UpsertDetail(taskId, detailId, updateDto);
            }

            Entities.Detail updatedDetailEntity = _Mapper.Map<Entities.Detail>(updateDto);

            Entities.Detail resultDetail = _Repo.UpdateDetail(taskId, detailId, updatedDetailEntity);

            //var resultDto = _Mapper.Map<DetailDTO>(resultDetail);
            //return Ok(resultDto);
            return NoContent();

        }

        /// <summary>
        /// PATCH partila update detail (with possible upsert)
        /// </summary>
        [HttpPatch("{detailId}")]
        public ActionResult PartialUpdateDetail(Guid taskId,
            Guid detailId,
            JsonPatchDocument<Models.DetailForUpdatingDTO> patchDocument)
        {
            if (!_Repo.TaskExists(taskId))
            {
                return NotFound();
            }

            Entities.Detail detail = _Repo.GetDetail(taskId, detailId);
            if (detail == null)
            {
                //return NotFound();

                //here is UPSERT code if we want to - in this case yes
                var detailDto = new DetailForUpdatingDTO();
                patchDocument.ApplyTo(detailDto, ModelState);
                if (!TryValidateModel(detailDto))
                {
                    return ValidationProblem(ModelState);
                }

                return UpsertDetail(taskId, detailId, detailDto);
            }

            //map actual detail to "forUpdateDTO"
            var detailToPatch = _Mapper.Map<DetailForUpdatingDTO>(detail);

            //apply patch (this is created no matter if is valid or not)
            patchDocument.ApplyTo(detailToPatch, ModelState); //if we pass modelstate all errors goes there

            //validate patched object and this also checks patch.applyTo when modelstate provided
            if (!TryValidateModel(detailToPatch))
            {
                return ValidationProblem(ModelState);
            }

            //from now it is same as PUT
            var updatedDetail = _Mapper.Map<Entities.Detail>(detailToPatch);

            Entities.Detail resultDetail = _Repo.UpdateDetail(taskId, detailId, updatedDetail);

            //var resultDto = _Mapper.Map<DetailDTO>(resultDetail);
            //return Ok(resultDto);
            return NoContent();
        }

        /// <summary>
        /// DELETE detail
        /// </summary>
        [HttpDelete("{detailId}")]
        public ActionResult DeleteDetail(Guid taskId, Guid detailId)
        {
            if (!_Repo.TaskExists(taskId))
            {
                return NotFound();
            }

            Entities.Detail detail = _Repo.GetDetail(taskId, detailId);
            if (detail == null)
            {
                return NotFound();
            }

            _Repo.DeleteDetail(detail);

            return NoContent();
        }

        /// <summary>
        /// UPSERT code for creating detail by PUT or PATCH
        /// </summary>
        /// <returns>201 created at route</returns>
        private ActionResult UpsertDetail(Guid taskId, Guid detailId, DetailForUpdatingDTO detailDto)
        {
            var detailToAdd = _Mapper.Map<Entities.Detail>(detailDto);
            detailToAdd.Id = detailId;

            var addedDetail = _Repo.CreateDetailwithId(taskId, detailId, detailToAdd);

            var detailToReturn = _Mapper.Map<DetailDTO>(addedDetail);
            return CreatedAtRoute(
                GetDetailRoute,
                new { taskId = taskId, detailId = detailToReturn.Id },
                detailToReturn
                );
        }

        /// <summary>
        /// overriding validateProblem
        /// </summary>
        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
