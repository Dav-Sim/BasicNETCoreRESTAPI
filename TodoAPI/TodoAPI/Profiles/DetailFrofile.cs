using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Profiles
{
    public class DetailFrofile : Profile
    {
        public DetailFrofile()
        {
            //create map from ENTITY to DTO
            CreateMap<Entities.Detail, Models.DetailDTO>();
            CreateMap<Entities.Detail, Models.DetailForUpdatingDTO>();


            //create map from DTO to ENTITY
            CreateMap<Models.DetailDTO, Entities.Detail>();
            CreateMap<Models.DetailForUpdatingDTO, Entities.Detail>();
            CreateMap<Models.DetailForCreatingDTO, Entities.Detail>();
        }
    }
}
