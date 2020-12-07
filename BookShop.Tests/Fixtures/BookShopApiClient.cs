using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookShop.Application.Features.Catalog;
using BookShop.Application.Features.OrderProcessing;
using BookShop.Application.Infrastructure;
using BookShop.Application.Infrastructure.Errors;
using FluentAssertions;
using Flurl;

namespace BookShop.Tests.Fixtures
{
    public class BookShopApiClient
    {
        private readonly HttpClient _httpClient;

        public BookShopApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BookModel> AddBook(AddBookCommand command)
        {
            using var response = await _httpClient.PostAsJsonAsync("books", command);
            var book = await SafelyReadFromJsonAsync<BookModel>(response, HttpStatusCode.Created);
            var expectedBook = new BookModel
            {
                Title = book.Title?.Trim(),
                Author = book.Author?.Trim(),
                Price = command.Price,
                Genre = command.Genre
            };
            book.Should()
                .BeEquivalentTo(expectedBook, o => o.Excluding(x => x.Id));
            return book;
        }

        public async Task<BookModel> GetBook(GetBookQuery query)
        {
            var url = $"books/{query.BookId}";
            using var response = await _httpClient.GetAsync(url);
            var book = await SafelyReadFromJsonAsync<BookModel>(response);
            book.Id.Should().Be(query.BookId);
            return book;
        }

        public async Task<CartModel> AddBookToCart(AddBookToCartCommand command)
        {
            var url = command.CartId == null 
                ? "carts/empty/books" 
                : $"carts/{command.CartId.Value}/books";
            using var response = await _httpClient.PostAsJsonAsync(url, command);
            var cart = await SafelyReadFromJsonAsync<CartModel>(
                response, 
                HttpStatusCode.Created, 
                HttpStatusCode.OK);
            cart.Id.Should().NotBeEmpty();
            if (command.CartId == null)
            {
                cart.Books.Should().HaveCount(1);
            }
            else
            {
                cart.Id.Should().Be(command.CartId.Value);
                cart.Books.Should().HaveCountGreaterOrEqualTo(1);
            }
            return cart;
        }

        public async Task<Page<BookModel>> SearchBooks(SearchBooksQuery query)
        {
            var url = new Url("books");
            if (query.Genres.Count > 0)
            {
                url.SetQueryParam(nameof(query.Genres).ToLowerInvariant(), query.Genres);
            }
            if (query.MaxPrice != null)
            {
                url.SetQueryParam(nameof(query.MaxPrice).ToLowerInvariant(), query.MaxPrice);
            }
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                url.SetQueryParam(nameof(query.SearchTerm).ToLowerInvariant(), query.SearchTerm);
            }
            using var response = await _httpClient.GetAsync(url);
            var booksPage = await SafelyReadFromJsonAsync<Page<BookModel>>(response);
            return booksPage;
        }
        
        public async Task<CartModel> GetCart(GetCartQuery query)
        {
            var url = $"carts/{query.CartId}";
            var response = await _httpClient.GetAsync(url);
            var cart = await SafelyReadFromJsonAsync<CartModel>(response);
            cart.Id.Should().Be(query.CartId);
            return cart;
        }
        
        public async Task<OrderModel> SubmitOrder(SubmitOrderCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync("orders", command);
            var order = await SafelyReadFromJsonAsync<OrderModel>(response);
            order.Id.Should().Be(command.CartId);
            order.Lines.Should().HaveCountGreaterOrEqualTo(1);
            return order;
        }

        private async Task<T> SafelyReadFromJsonAsync<T>(HttpResponseMessage response,
            params HttpStatusCode[] validSuccessStatusCodes)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.Content.Headers.ContentType?.MediaType?.Contains("application/problem+json") == true)
                {
                    var validationError = await response.Content.ReadFromJsonAsync<ValidationError>();
                    validationError.Should().NotBeNull("validation error content should be present in the body");
                    throw validationError!.ToException();
                }
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"Unexpected response with status code {response.StatusCode}. {error}");
            }
            if (validSuccessStatusCodes.Length > 0)
            {
                validSuccessStatusCodes.Should().Contain(response.StatusCode);
            }
            else
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}