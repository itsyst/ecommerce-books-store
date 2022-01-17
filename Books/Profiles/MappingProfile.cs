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
            CreateMap<CategoryDto, CategoryDto>();
            CreateMap<Cover, CoverDto>();
            CreateMap<AuthorDto, AuthorDto>();


            // Dto to Domain
            CreateMap<ProductDto, Product>()
                .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<CategoryDto, Product>()
                 .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<CoverDto, Product>()
                .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<AuthorDto, Product>()
                 .ForMember(p => p.Id, opt => opt.Ignore());
        }
    }
}
