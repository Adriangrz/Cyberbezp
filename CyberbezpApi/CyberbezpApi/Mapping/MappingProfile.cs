using AutoMapper;
using CyberbezpApi.Database.Entities;
using CyberbezpApi.Models;

namespace CyberbezpApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoginDto, RequestTokenDto>();
            CreateMap<RegistrationDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
