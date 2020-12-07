using BookShop.Application.Features.Catalog;

namespace BookShop.Application.Features.OrderProcessing
{
    public class OrderLineModel
    {
        public BookModel Book { get; set; }
        public decimal Price { get; set; }
    }
}