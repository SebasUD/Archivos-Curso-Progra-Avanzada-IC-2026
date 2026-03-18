
using Microsoft.EntityFrameworkCore;
using MyLibrary.DbModel.Context;
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories
{
    public class BookCopyRepository : Repository<Bookcopy>, IBookCopyRepository
    {
        private readonly IBookRepository _bookRepository;

        public BookCopyRepository(MySqlDbContext context, IBookRepository bookRepository) : base(context)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Bookcopy>> GetByStatusAsync(int status)
        {
            return await _context.Bookcopies
                .Where(bc => bc.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bookcopy>> GetByIsbnAsync(string isbn)
        {
            var book = await _bookRepository.GetBookByIsbnAsync(isbn);
            if (book == null)
            {
                return new List<Bookcopy>();
            }
            return await _context.Bookcopies
                .Where(bc => bc.BookId == book.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bookcopy>> GetByBookPropertiesAsync(string title, string description, string publisher)
        {
            var books = await _bookRepository.FindBooksAsync(title, description, publisher);
            var bookIds = books.Select(b => b.Id);

            return await _context.Bookcopies
                .Where(bc => bookIds.Contains(bc.BookId))
                .ToListAsync();
        }
    }
}
