using AutoMapper;
using Books.Domain.Entities;
using Books.Dtos;
using Books.Models;

namespace Books.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to Dto
            CreateMap<Product, ProductDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Cover, CoverDto>();
            CreateMap<Author, AuthorDto>();
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<Company, CompanyDto>();
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<OrderHeader, OrderHeaderDto>();
            CreateMap<OrderDetail, OrderDetailDto>();


            // Dto to Domain
            CreateMap<ProductDto, Product>()
                .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<CategoryDto, Category>()
                 .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<CoverDto, Cover>()
                .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<AuthorDto, Author>()
                 .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<ApplicationUserDto, ApplicationUser>()
                 .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<CompanyDto, Company>()
                 .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<ShoppingCartDto, ShoppingCart>()
                 .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<OrderHeaderDto, OrderHeader>()
                .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<OrderDetailDto, OrderDetail>()
                .ForMember(p => p.Id, opt => opt.Ignore());
        }
    }
}
