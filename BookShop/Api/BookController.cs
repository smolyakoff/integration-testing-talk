using System;
using System.Threading.Tasks;
using BookShop.Application.Features;
using BookShop.Application.Features.Catalog;
using BookShop.Application.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Api
{
    [Route("books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<BookModel>> AddBook([FromBody] AddBookCommand command)
        {
            var book = await _mediator.Send(command);
            var bookUrl = Url.Action(nameof(GetBook), new {bookId = book.Id});
            return Created(bookUrl, book);
        }

        [HttpGet]
        [Route("{bookId:guid}")]
        public async Task<ActionResult<BookModel>> GetBook(Guid bookId)
        {
            var query = new GetBookQuery {BookId = bookId};
            var book = await _mediator.Send(query);
            return Ok(book);
        }

        [HttpGet]
        public async Task<ActionResult<Page<BookModel>>> SearchBooks([FromQuery] SearchBooksQuery query)
        {
            var booksPage = await _mediator.Send(query);
            return Ok(booksPage);
        }
    }
}