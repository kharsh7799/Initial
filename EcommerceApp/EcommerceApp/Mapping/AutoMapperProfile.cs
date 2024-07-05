using AutoMapper;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs;
using EcommerceApp.Entities.DTOs.Category;
using EcommerceApp.Entities.DTOs.Product;

namespace EcommerceApp.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //products
            CreateMap<Product, ProductResponseDTO>().ReverseMap();
            CreateMap<ProductRequestDTO, Product>().ReverseMap();

            //category
            CreateMap<CategoryRequestDTO, Category>().ReverseMap();
            CreateMap<Category, CategoryResponseDTO>().ReverseMap();

            CreateMap<Category, CategoryWithProductsResDTO>().ReverseMap();

        }
    }
}
