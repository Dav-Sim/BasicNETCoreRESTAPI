using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Profiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            //create map from ENTITY to DTO
            CreateMap<Entities.Task, Models.TaskDTO>()
                .ForMember(
                    dest => dest.Status,
                    opts => opts.MapFrom(src => src.Status.ToString()));
            CreateMap<Entities.Task, Models.TaskForUpdatingDTO>()
                .ForMember(
                    dest => dest.Status,
                    opts => opts.MapFrom(src => src.Status.ToString()));


            //create map from DTO to ENTITY
            CreateMap<Models.TaskDTO, Entities.Task>()
                .ForMember(
                    dest => dest.Status,
                    opts => opts.MapFrom(src => 
                        Enum.Parse(typeof(Entities.Status), src.Status.ToString())));

            CreateMap<Models.TaskForUpdatingDTO, Entities.Task>()
                .ForMember(
                    dest => dest.Status,
                    opts => opts.MapFrom(src =>
                        Enum.Parse(typeof(Entities.Status), src.Status.ToString())))
                .ForMember(
                    dest => dest.Details,
                    opts => opts.MapFrom(src => src.Details));

            CreateMap<Models.TaskForCreatingDTO, Entities.Task>()
                .ForMember(
                    dest => dest.Status,
                    opts => opts.MapFrom(src =>
                        Enum.Parse(typeof(Entities.Status), src.Status.ToString())));
        }
    }
}
