
using Microsoft.EntityFrameworkCore;
using MyLibrary.DbModel.Context;
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(MySqlDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
        {
            return await _context.Bookauthors
                .Where(ba => ba.AuthorId == authorId)
                .Select(ba => ba.Book)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByThemeAsync(int themeId)
        {
            return await _context.Bookthemes
                .Where(bt => bt.ThemeId == themeId)
                .Select(bt => bt.Book)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> FindBooksAsync(string title, string description, string publisher)
        {
            return await _context.Books
                .Where(b => (string.IsNullOrEmpty(title) || b.Title.Contains(title)) &&
                            (string.IsNullOrEmpty(description) || b.Description.Contains(description)) &&
                            (string.IsNullOrEmpty(publisher) || b.Publisher.Contains(publisher)))
                .ToListAsync();
        }

        public async Task<Book> GetBookByIsbnAsync(string isbn)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.Isbn == isbn);
        }
    }
}
