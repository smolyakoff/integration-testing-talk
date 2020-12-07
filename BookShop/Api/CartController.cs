using System;
using System.Threading.Tasks;
using BookShop.Application.Features.OrderProcessing;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Api
{
    [ApiController]
    [Route("carts")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        [Route("empty/books")]
        public async Task<ActionResult<CartModel>> AddBookToEmptyCart([FromBody] AddBookToCartCommand command)
        {
            var cart = await _mediator.Send(command);
            var url = Url.Action(nameof(GetCart), new {cartId = cart.Id});
            return Created(url, cart);
        }
        
        [HttpPost]
        [Route("{cartId:guid}/books")]
        public async Task<ActionResult<CartModel>> AddBookToCart(Guid cartId, [FromBody] AddBookToCartCommand command)
        {
            command.CartId = cartId;
            var cart = await _mediator.Send(command);
            return Ok(cart);
        }

        [HttpGet]
        [Route("{cartId:guid}")]
        public async Task<ActionResult<CartModel>> GetCart(Guid cartId)
        {
            var query = new GetCartQuery {CartId = cartId};
            var cart = await _mediator.Send(query);
            return cart;
        }
    }
}