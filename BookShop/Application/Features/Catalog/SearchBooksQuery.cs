using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookShop.Application.Infrastructure;
using BookShop.Application.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Features.Catalog
{
    public class SearchBooksQuery : IRequest<Page<BookModel>>
    {
        public string SearchTerm { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<BookGenre> Genres { get; set; }
    }

    internal class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, Page<BookModel>>
    {
        private readonly BookShopDbContext _dbContext;
        private readonly IMapper _mapper;

        public SearchBooksQueryHandler(BookShopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Page<BookModel>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
        {
            var query = _mapper.ProjectTo<BookModel>(_dbContext.Books);
            if (request.Genres.Any())
            {
                var uniqueGenres = request.Genres.Distinct();
                query = query.Where(x => uniqueGenres.Contains(x.Genre));
            }

            if (request.MaxPrice != null)
            {
                query = query.Where(x => x.Price <= request.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var normalizedSearchTerm = request.SearchTerm.ToLowerInvariant();
                query = query.Where(x =>
                    x.Title.ToLower().Contains(normalizedSearchTerm) ||
                    x.Author.ToLower().Contains(normalizedSearchTerm));
            }

            var items = await query
                .OrderBy(x => x.Price)
                .ToListAsync(cancellationToken);
            return new Page<BookModel>(items);
        }
    }
}