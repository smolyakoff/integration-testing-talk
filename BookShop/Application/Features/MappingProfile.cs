using AutoMapper;
using BookShop.Application.Features.Catalog;
using BookShop.Application.Features.OrderProcessing;

namespace BookShop.Application.Features
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookModel>();
            CreateMap<Cart, CartModel>();
            CreateMap<Order, OrderModel>();
            CreateMap<OrderLine, OrderLineModel>();
        }
    }
}