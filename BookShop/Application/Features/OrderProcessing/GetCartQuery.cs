using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookShop.Application.Infrastructure;
using BookShop.Application.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Features.OrderProcessing
{
    public class GetCartQuery : IRequest<CartModel>
    {
        public Guid CartId { get; set; }
    }

    internal class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartModel>
    {
        private readonly BookShopDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetCartQueryHandler(BookShopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<CartModel> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = 
                await _dbContext.Carts
                    .Include(x => x.Books)
                    .SingleOrDefaultAsync(x => x.Id == request.CartId, cancellationToken) ??
                new Cart(request.CartId);
            return _mapper.Map<CartModel>(cart);
        }
    }
}