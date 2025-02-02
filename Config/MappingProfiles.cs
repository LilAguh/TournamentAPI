
using AutoMapper;
using Core.DTOs;
using Models.DTOs;
using Models.Entities;

namespace Config
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<PlayerRegisterDto, User>();
            CreateMap<AdminRegisterDto, User>();
            CreateMap<UpdateUserDto, User>();

            CreateMap<User, UserResponseDto>();
        }
    }
}
