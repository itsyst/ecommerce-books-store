using AutoMapper;
using Books.Domain.Entities;
using Books.Dtos;

namespace Books.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to Dto
            CreateMap<Product, ProductDto>();


            // Dto to Domain
            CreateMap<ProductDto, Product>()
                .ForMember(p => p.Id, opt => opt.Ignore());
        }
    }
}
