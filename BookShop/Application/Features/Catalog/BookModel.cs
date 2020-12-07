using System;

namespace BookShop.Application.Features.Catalog
{
    public class BookModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public BookGenre Genre { get; set; }
        public decimal Price { get; set; }
    }
}