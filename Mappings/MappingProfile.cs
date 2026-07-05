using AutoMapper;
using InternalRequestSystem.Models;

namespace InternalRequestSystem.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Request, Request>();
        }
    }
}