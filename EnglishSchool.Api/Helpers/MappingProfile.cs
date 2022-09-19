using AutoMapper;
using EnglishSchool.Common.Dtos;
using EnglishSchool.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishSchool.Common.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Material, MaterialDto>();
            CreateMap<MaterialDto, Material>();
            CreateMap<LevelDto, Level>();
            CreateMap<Level, LevelDto>();
        }
    }
}
