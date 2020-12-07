using System;
using System.Collections.Generic;
using BookShop.Application.Features.Catalog;

namespace BookShop.Application.Features.OrderProcessing
{
    public class Cart
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private HashSet<Book> _books;

        public Cart(Guid cartId) : this()
        {
            Id = cartId;
        }
        
        public Cart(Book book, Guid? cartId) : this()
        {
            Id = cartId ?? Guid.NewGuid();
            _books.Add(book);
        }
        
        protected Cart()
        {
            _books = new HashSet<Book>();
        }

        public Guid Id { get; private set; }
        public IReadOnlyCollection<Book> Books => _books;

        public void AddBook(Book book)
        {
            _books.Add(book);
        }
    }
}