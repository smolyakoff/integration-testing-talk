using System;
using System.Collections.Generic;
using BookShop.Application.Features.Catalog;

namespace BookShop.Application.Features.OrderProcessing
{
    public class CartModel
    {
        public Guid Id { get; set; }
        
        public List<BookModel> Books { get; set; }
    }
}