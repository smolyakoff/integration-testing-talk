using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookShop.Application.Infrastructure.Database;
using BookShop.Application.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Features.OrderProcessing
{
    public class SubmitOrderCommand : IRequest<OrderModel>
    {
        public Guid CartId { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }
    }

    internal class SubmitOrderCommandHandler : IRequestHandler<SubmitOrderCommand, OrderModel>
    {
        private readonly BookShopDbContext _dbContext;
        private readonly IMapper _mapper;

        public SubmitOrderCommandHandler(BookShopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<OrderModel> Handle(SubmitOrderCommand request, CancellationToken cancellationToken)
        {
            var cart = await _dbContext.Carts
                .Include(x => x.Books)
                .SingleOrDefaultAsync(
                x => x.Id == request.CartId, cancellationToken);
            if (cart == null)
            {
                throw NotFoundError.CreateForResource(nameof(Cart), request.CartId).ToException();
            }
            var order = new Order(cart, request.PhoneNumber);
            await _dbContext.AddAsync(order, cancellationToken);
            _dbContext.Remove(cart);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<OrderModel>(order);
        }
    }
}