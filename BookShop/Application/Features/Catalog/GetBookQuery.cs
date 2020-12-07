using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookShop.Application.Infrastructure;
using BookShop.Application.Infrastructure.Database;
using BookShop.Application.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Features.Catalog
{
    public class GetBookQuery : IRequest<BookModel>
    {
        public Guid BookId { get; set; }
    }

    internal class GetBookQueryHandler : IRequestHandler<GetBookQuery, BookModel>
    {
        private readonly BookShopDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetBookQueryHandler(BookShopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Task<BookModel> Handle(GetBookQuery request, CancellationToken cancellationToken)
        {
            var book = _mapper
                .ProjectTo<BookModel>(_dbContext.Books)
                .FirstOrDefaultAsync(x => x.Id == request.BookId, cancellationToken);
            if (book == null)
            {
                throw NotFoundError.CreateForResource(nameof(Book), request.BookId).ToException();
            }

            return book;
        }
    }
}