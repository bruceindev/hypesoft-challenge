using AutoMapper;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Application.Categories.DTOs;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.PriceCurrency, opt => opt.MapFrom(src => src.Price.Currency))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity.Value))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.StockQuantity.IsLowStock()));

        CreateMap<Category, CategoryResponseDto>();
    }
}
