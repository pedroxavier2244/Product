using AutoMapper;
using Product.Application.DTOs;

namespace Product.Application.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Domain.Models.Product, ProductDto>();
            CreateMap<CreateProductDto, Domain.Models.Product>();
            CreateMap<UpdateProductDto, Domain.Models.Product>();
        }
    }
}