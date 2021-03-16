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
            CreateMap<Entities.Task, Models.TaskSimpleDto>()
                .ForMember(dest => dest.Text,
                opts => opts.MapFrom(src => $"{src.Name}-{src.Description}-{src.Priority}-{src.Status}"));


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
                        Enum.Parse(typeof(Entities.Status), src.Status.ToString())));
            CreateMap<Models.TaskForCreatingDTO, Entities.Task>()
                .ForMember(
                    dest => dest.Status,
                    opts => opts.MapFrom(src =>
                        Enum.Parse(typeof(Entities.Status), src.Status.ToString())));
        }
    }
}
