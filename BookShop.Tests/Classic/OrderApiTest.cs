using System.Linq;
using System.Threading.Tasks;
using BookShop.Application.Features.Catalog;
using BookShop.Application.Features.OrderProcessing;
using BookShop.Application.Infrastructure.Errors;
using BookShop.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace BookShop.Tests.Classic
{
    public class OrderApiTest : BaseApiTest
    {
        public OrderApiTest(BookShopFixture fixture) 
            : base(fixture)
        {
        }
        
        [Fact]
        public async Task SubmitOrder_CreatesAnOrderSuccessfully()
        {
            // Set up data through the back door
            var book1 = new Book(
                BookGenre.Detective, 
                "No Orchids for Miss Blandish", 
                "James Hadley Chase", 
                10m);
            var book2 = new Book(
                BookGenre.Detective, 
                "Come Easy – Go Easy", 
                "James Hadley Chase", 
                15m);
            var cart = new Cart(book1, null);
            cart.AddBook(book2);
            DbContext.Add(cart);
            await DbContext.SaveChangesAsync();
            
            // Set up data through the front door
            // Nothing here, did everything through the back door :)
            
            // Build inputs
            var command = new SubmitOrderCommand
            {
                CartId = cart.Id, 
                PhoneNumber = "+375291234567"
            };
            
            // Send inputs to system
            var actualOrder = await Client.SubmitOrder(command);
            
            // Verify direct outputs
            actualOrder.Id.Should().NotBeEmpty();
            var expectedOrder = new OrderModel
            {
                Id = actualOrder.Id,
                PhoneNumber = command.PhoneNumber,
                Lines = cart.Books.Select(book => new OrderLineModel
                {
                    Book = new BookModel
                    {
                        Id = book.Id,
                        Author = book.Author,
                        Genre = book.Genre,
                        Price = book.Price,
                        Title = book.Title
                    },
                    Price = book.Price
                }).ToList()
            };
            actualOrder.Should().BeEquivalentTo(expectedOrder);
            
            // Verify side-effects
            cart = DbContext.Carts.SingleOrDefault(x => x.Id == cart.Id);
            cart.Should().BeNull("cart should be deleted after order is submitted");
        }

        [Fact]
        public async Task SubmitOrder_WithInvalidInput_ReturnsValidationError()
        {
            var book = new Book(
                BookGenre.Detective, 
                "No Orchids for Miss Blandish", 
                "James Hadley Chase", 
                10m);
            var cart = new Cart(book, null);
            DbContext.Add(cart);
            await DbContext.SaveChangesAsync();

            var command = new SubmitOrderCommand
            {
                CartId = cart.Id,
                PhoneNumber = null
            };
            var exception = await Client.Invoking(x => x.SubmitOrder(command))
                .Should()
                .ThrowAsync<ServiceException>();
            var error = exception.Which.Error.Should().BeOfType<ValidationError>().Subject;
            error.Errors.Should().ContainKey(nameof(command.PhoneNumber));
        }
    }
}