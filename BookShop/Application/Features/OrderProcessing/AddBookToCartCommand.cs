using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookShop.Application.Features.Catalog;
using BookShop.Application.Infrastructure;
using BookShop.Application.Infrastructure.Database;
using BookShop.Application.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Features.OrderProcessing
{
    public class AddBookToCartCommand : IRequest<CartModel>
    {
        [JsonIgnore]
        public Guid? CartId { get; set; }
        public Guid BookId { get; set; }
    }

    internal class AddBookToCartCommandHandler : IRequestHandler<AddBookToCartCommand, CartModel>
    {
        private readonly BookShopDbContext _dbContext;
        private readonly IMapper _mapper;

        public AddBookToCartCommandHandler(BookShopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<CartModel> Handle(AddBookToCartCommand request, CancellationToken cancellationToken)
        {
            var book = await _dbContext.Books.SingleOrDefaultAsync(
                x => x.Id == request.BookId, 
                cancellationToken);
            if (book == null)
            {
                throw NotFoundError.CreateForResource(nameof(Book), request.BookId).ToException();
            }
            Cart cart = null;
            if (request.CartId != null)
            {
                cart = await _dbContext.Carts.Include(x => x.Books).SingleOrDefaultAsync(
                    x => x.Id == request.CartId.Value,
                    cancellationToken);
            }
            if (cart == null)
            {
                cart = new Cart(book, request.CartId);
                await _dbContext.Carts.AddAsync(cart, cancellationToken);
            }
            else
            {
                cart.AddBook(book);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            var cartModel = _mapper.Map<CartModel>(cart);
            return cartModel;
        }
    }
}