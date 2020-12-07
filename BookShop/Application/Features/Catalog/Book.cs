using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace BookShop.Application.Features.Catalog
{
    public class Book
    {
        protected Book()
        {
        }
        
        public Book(BookGenre genre, string title, string author, decimal price)
        {
            Id = Guid.NewGuid();
            Genre = genre;
            Title = title.Trim();
            Author = author.Trim();
            Price = price;
        }

        public Guid Id { get; private set; }
        public BookGenre Genre { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public decimal Price { get; private set; }
    }
}