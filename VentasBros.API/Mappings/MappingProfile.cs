using AutoMapper;
using VentasBros.Domain.Entities;
using VentasBros.Application.DTOs.Product;
using VentasBros.Application.DTOs.Category;

namespace VentasBros.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Image mappings
            CreateMap<Image, ImageDto>();

            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
        }
    }
}
