using AutoMapper;
using PloomesAPI.Data.Dtos;
using PloomesAPI.Models;
using StoreAPI.Model;

namespace PloomesAPI.Profiles
{
    public class ItemCategoryProfile : Profile
    {
        public ItemCategoryProfile()
        {
            CreateMap<CreateCategoryDTO, ItemCategory>();
            CreateMap<ItemCategory, ReadCategoryDTO>();
            CreateMap<UpdateCategoryDTO, ItemCategory>();
        }
    }
}
