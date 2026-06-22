using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Common.Mappings;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt, options => options.Ignore())
            .ForMember(destination => destination.DomainEvents, options => options.Ignore());
        CreateMap<User, CreateUserResult>();
        CreateMap<User, GetUserResult>()
            .ForMember(destination => destination.Name, options => options.MapFrom(source => source.Username));

        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.TotalAmount, options => options.Ignore())
            .ForMember(destination => destination.IsCancelled, options => options.Ignore())
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt, options => options.Ignore())
            .ForMember(destination => destination.DomainEvents, options => options.Ignore());

        CreateMap<CreateSaleItemCommand, SaleItem>()
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.SaleId, options => options.Ignore())
            .ForMember(destination => destination.DiscountPercentage, options => options.Ignore())
            .ForMember(destination => destination.DiscountAmount, options => options.Ignore())
            .ForMember(destination => destination.TotalAmount, options => options.Ignore())
            .ForMember(destination => destination.IsCancelled, options => options.Ignore())
            .ForMember(destination => destination.DomainEvents, options => options.Ignore());

        CreateMap<UpdateSaleItemCommand, SaleItem>()
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.SaleId, options => options.Ignore())
            .ForMember(destination => destination.DiscountPercentage, options => options.Ignore())
            .ForMember(destination => destination.DiscountAmount, options => options.Ignore())
            .ForMember(destination => destination.TotalAmount, options => options.Ignore())
            .ForMember(destination => destination.IsCancelled, options => options.Ignore())
            .ForMember(destination => destination.DomainEvents, options => options.Ignore());

        CreateMap<Sale, GetSaleResult>();
        CreateMap<Sale, CreateSaleResult>();
        CreateMap<Sale, UpdateSaleResult>();
        CreateMap<Sale, ListSalesResult>();
        CreateMap<SaleItem, GetSaleItemResult>();
    }
}
