using System;
using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace BookShop.Application.Features.OrderProcessing
{
    public class Order
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private List<OrderLine> _lines;
        
        public Order(Cart cart, string phoneNumber)
        {
            if (cart.Books.Count == 0)
            {
                throw new ArgumentException("Can't create orders from empty carts.", nameof(cart));
            }
            Id = cart.Id;
            _lines = new List<OrderLine>();
            foreach (var book in cart.Books)
            {
                var orderLine = new OrderLine(Id, book);
                _lines.Add(orderLine);
            }
            PhoneNumber = phoneNumber;
        }

        protected Order()
        {
        }
        
        public Guid Id { get; private set; }
        public string PhoneNumber { get; private set; }
        
        public IReadOnlyCollection<OrderLine> Lines => _lines;
    }
}