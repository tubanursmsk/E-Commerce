using AutoMapper;
using ECommerce.Domain.Entities;
using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.DTOs.Company;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.DTOs.Customer;
using ECommerce.Application.DTOs.Cargo;
using ECommerce.Application.DTOs.Banner;
using ECommerce.Application.DTOs.Review;

namespace ECommerce.Application.Mappings;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        // Product Mappings
        CreateMap<Product, ProductDto>()
        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
        .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name)); //ProductDto içinde CategoryName diye bir alan var ama Product entity'sinde bu yok (orada sadece Category nesnesi var). .ForMember(...) satırı ile AutoMapper'a: "Category nesnesinin içindeki Name'i al, DTO'daki CategoryName'e yaz" demiş oluyoruz.
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();
        CreateMap<ProductDeleteDto, Product>();

        // Category Mappings
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();
        CreateMap<CategoryDeleteDto, Category>();

        // Company Mappings
        CreateMap<Company, CompanyDto>().ReverseMap(); //AutoMapper içinde, tanımlanan yönün tersine (destination(hedef) -> source(kaynak)) otomatik bir eşleme (mapping) daha oluşturur.
        CreateMap<CompanyCreateDto, Company>();
        CreateMap<CompanyUpdateDto, Company>();
        CreateMap<CompanyDeleteDto, Company>();

        // Auth/User Mappings
        CreateMap<RegisterDto, User>();
        CreateMap<RegisterCompanyDto, User>();
        CreateMap<LoginDto, User>();
        CreateMap<User, AuthResponseDto>();

        //User -Admin- Role Mappings
        CreateMap<User, UserDto>();
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => new List<string> { src.Role }));

        // Order Mappings
        CreateMap<Order, OrderSummaryDto>()
            .ForMember(dest => dest.CustomerFullName, opt => opt.MapFrom(src => $"{src.Customer.User.FirstName} {src.Customer.User.LastName}"));
        CreateMap<OrderDto, Order>().ReverseMap(); // Hem Order -> OrderDto hem de tersini yapar
        CreateMap<OrderItem, OrderItemDto>().ReverseMap(); // Sipariş kalemleri için de gerekli
        CreateMap<OrderCreateDto, Order>();
        CreateMap<OrderItemCreateDto, OrderItem>();
        CreateMap<OrderUpdateDto, Order>();
        CreateMap<OrderItemUpdateDto, OrderItem>();
        CreateMap<OrderDeleteDto, Order>();

        // Brand Mappings
        CreateMap<Brand, BrandDto>().ReverseMap();
        CreateMap<BrandCreateDto, Brand>();
        CreateMap<BrandUpdateDto, Brand>();
        CreateMap<BrandDeleteDto, Brand>();

        // Customer Mappings
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
        CreateMap<CustomerDto, Customer>();
        CreateMap<CustomerCreateDto, Customer>();
        CreateMap<CustomerUpdateDto, Customer>();
        CreateMap<CustomerDeleteDto, Customer>();

        // Cargo Mappings
        CreateMap<Cargo, CargoDto>().ReverseMap();
        CreateMap<CargoCreateDto, Cargo>();
        CreateMap<CargoUpdateDto, Cargo>();
        CreateMap<CargoDeleteDto, Cargo>();

        //Banner Mappings
        CreateMap<Banner, BannerDto>().ReverseMap();
        CreateMap<BannerCreateDto, Banner>();
        CreateMap<BannerUpdateDto, Banner>();
        CreateMap<BannerDeleteDto, Banner>();

        // Review Mappings
        CreateMap<Review, ReviewDto>().ReverseMap();
        CreateMap<ReviewCreateDto, Review>();
        CreateMap<ReviewUpdateDto, Review>();
        CreateMap<ReviewDeleteDto, Review>();



        // devamı eklenecek


    }
}