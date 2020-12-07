using System;
using BookShop.Application.Features.Catalog;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace BookShop.Application.Features.OrderProcessing
{
    public class OrderLine
    {
        private Guid _bookId;
        
        public OrderLine(Guid orderId, Book book)
        {
            OrderId = orderId;
            _bookId = book.Id;
            Book = book;
            Price = book.Price;
        }

        protected OrderLine()
        {
        }
        
        public Guid OrderId { get; private set; }
        public Book Book { get; private set; }
        public decimal Price { get; private set; }
    }
}