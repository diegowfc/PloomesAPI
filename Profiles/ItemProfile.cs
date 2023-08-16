using AutoMapper;
using PloomesAPI.Data.Dtos;
using StoreAPI.Model;

namespace PloomesAPI.Profiles
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<CreateItemDTO, Item>();
            CreateMap<UpdateItemDTO, Item>();
            CreateMap<Item, UpdateItemDTO>();
            CreateMap<Item, ReadItemDTO>();
        }
    }
}
