using System.Threading.Tasks;
using BookShop.Application.Features.OrderProcessing;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Api
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        public async Task<ActionResult<OrderModel>> SubmitOrder([FromBody] SubmitOrderCommand command)
        {
            var order = await _mediator.Send(command);
            return order;
        }
    }
}