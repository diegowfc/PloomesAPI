using AutoMapper;
using PloomesAPI.Data.Dtos;
using StoreAPI.Model;

namespace PloomesAPI.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, ReadUserDTO>().ForMember(userDTO => userDTO.Role, opt => opt.MapFrom(role => role.Role));
        }
    }
}
