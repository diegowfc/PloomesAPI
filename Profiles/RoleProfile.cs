using AutoMapper;
using PloomesAPI.Data.Dtos;
using PloomesAPI.Models;

namespace PloomesAPI.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<CreateRoleDTO, Role>();
            CreateMap<UpdateRoleDTO, Role>();
            CreateMap<Role, ReadRoleDTO>();
        }
    }
}
