using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Application.Features.Catalog;
using BookShop.Application.Features.OrderProcessing;
using BookShop.Application.Infrastructure.Errors;
using BookShop.Tests.Fixtures;
using BookShop.Tests.Framework;
using FluentAssertions;
using Xunit.Abstractions;

namespace BookShop.Tests.Scenario
{
    public class CustomerMakesAnOrderScenario : BaseScenario
    {
        private static List<BookModel> _books;
        private static List<BookModel> _selectedBooks;
        private static CartModel _cart;
        
        public CustomerMakesAnOrderScenario(BookShopFixture fixture) 
            : base(fixture)
        {
        }
        
        [Step]
        public async Task Step1__Supplier_Adds_Books_To_Catalog()
        {
            var booksToAdd = new List<AddBookCommand>
            {
                new AddBookCommand
                {
                    Title = "Casino Royale",
                    Author = "Ian Fleming",
                    Genre = BookGenre.Detective,
                    Price = 20.5m
                },
                new AddBookCommand
                {
                    Title = "Diamonds Are Forever",
                    Author = "Ian Fleming",
                    Genre = BookGenre.Detective,
                    Price = 15.25m
                },
                new AddBookCommand
                {
                    Title = "I, Robot",
                    Author = "Isaac Asimov",
                    Genre = BookGenre.ScienceFiction,
                    Price = 20.05m
                },
                new AddBookCommand
                {
                    Title = "Soul of a Robot",
                    Author = "Barrington J. Bayley",
                    Genre = BookGenre.ScienceFiction,
                    Price = 15.05m
                },
                new AddBookCommand
                {
                    Title = "War with the Robots",
                    Author = "Harry Harrison",
                    Genre = BookGenre.ScienceFiction,
                    Price = 10m,
                }
            };
            _books = new List<BookModel>();
            foreach (var book in booksToAdd)
            {
                var addedBook = await Client.AddBook(book);
                _books.Add(addedBook);
            }
        }

        [Step]
        public async Task Step2__Customer_Searches_For_Cheap_Science_Fiction_Books_About_Robots()
        {
            const decimal maxPrice = 20m;
            var query = new SearchBooksQuery
            {
                Genres = new List<BookGenre> {BookGenre.ScienceFiction},
                SearchTerm = "robot",
                MaxPrice = maxPrice
            };
            var expectedBooks =
                _books.Where(
                    x => x.Title.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) && 
                         x.Price <= maxPrice);
            var booksPage = await Client.SearchBooks(query);
            booksPage.Items.Should().BeEquivalentTo(expectedBooks);
            booksPage.Items.Should().BeInAscendingOrder(x => x.Price);
            _selectedBooks = booksPage.Items;
        }

        [Step]
        public async Task Step3__Customer_Adds_Books_To_Cart()
        {
            var firstBook = _selectedBooks.First();
            var addFirstBookCommand = new AddBookToCartCommand
            {
                BookId = firstBook.Id,
                CartId = null
            };
            var cart = await Client.AddBookToCart(addFirstBookCommand);
            cart.Id.Should().NotBeEmpty();
            var expectedCart = new CartModel
            {
                Id = cart.Id,
                Books = new List<BookModel> {firstBook}
            };
            cart.Should().BeEquivalentTo(expectedCart);
            var subsequentBooks = _selectedBooks.Skip(1);
            foreach (var book in subsequentBooks)
            {
                var addSubsequentBookCommand = new AddBookToCartCommand
                {
                    BookId = book.Id,
                    CartId = cart.Id
                };
                cart = await Client.AddBookToCart(addSubsequentBookCommand);
            }
            // Testing idempotency just in case UI does something weird
            cart = await Client.AddBookToCart(new AddBookToCartCommand
            {
                BookId = firstBook.Id,
                CartId = cart.Id,
            });
            var expectedFinalCart = new CartModel
            {
                Id = cart.Id,
                Books = _selectedBooks
            };
            cart.Should().BeEquivalentTo(expectedFinalCart);
            _cart = cart;
        }

        [Step]
        public async Task Step4__Customer_Tries_To_Submit_An_Order_Without_Phone_Number()
        {
            // Visit the cart page and check that books are still there
            var cartQuery = new GetCartQuery {CartId = _cart.Id};
            var cart = await Client.GetCart(cartQuery);
            cart.Should().BeEquivalentTo(_cart);
            
            // Submit order
            var submitOrderCommand = new SubmitOrderCommand
            {
                PhoneNumber = null,
                CartId = _cart.Id
            };
            var exception = await Client.Invoking(x => x.SubmitOrder(submitOrderCommand))
                .Should()
                .ThrowAsync<ServiceException>();
            var error = exception.Which.Error.Should().BeOfType<ValidationError>().Subject;
            error.Errors.Should().ContainKey(nameof(submitOrderCommand.PhoneNumber));
        }
        
        [Step]
        public async Task Step5__Customer_Submits_A_Valid_Order()
        {
            // Submit order
            var submitOrderCommand = new SubmitOrderCommand
            {
                PhoneNumber = "+375291234567",
                CartId = _cart.Id
            };
            var order = await Client.SubmitOrder(submitOrderCommand);
            var expectedOrder = new OrderModel
            {
                Id = _cart.Id,
                PhoneNumber = submitOrderCommand.PhoneNumber,
                Lines = _selectedBooks.Select(book => new OrderLineModel
                {
                    Book = book,
                    Price = book.Price
                }).ToList()
            };
            order.Should().BeEquivalentTo(expectedOrder);
            
            // Cart should become empty
            var cartQuery = new GetCartQuery {CartId = _cart.Id};
            var cart = await Client.GetCart(cartQuery);
            cart.Books.Should().BeEmpty();
        }
    }
}