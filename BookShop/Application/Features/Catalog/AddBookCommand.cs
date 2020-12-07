using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookShop.Application.Infrastructure.Database;
using MediatR;

namespace BookShop.Application.Features.Catalog
{
    public class AddBookCommand : IRequest<BookModel>
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public BookGenre Genre { get; set; }
        public decimal Price { get; set; }
    }

    internal class AddBookCommandHandler : IRequestHandler<AddBookCommand, BookModel>
    {
        private readonly BookShopDbContext _dbContext;
        private readonly IMapper _mapper;

        public AddBookCommandHandler(BookShopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<BookModel> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            var book = new Book(request.Genre, request.Title, request.Author, request.Price);
            await _dbContext.Books.AddAsync(book, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            var bookModel = _mapper.Map<BookModel>(book);
            return bookModel;
        }
    }
}