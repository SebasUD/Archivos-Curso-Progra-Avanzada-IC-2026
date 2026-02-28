
using Microsoft.EntityFrameworkCore;
using MyLibrary.DbModel.Context;
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories
{
    public class BookThemeRepository : Repository<Booktheme>, IBookThemeRepository
    {
        public BookThemeRepository(MySqlDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Booktheme>> GetByPartialBookNameAsync(string bookName)
        {
            return await _context.Bookthemes
                .Include(bt => bt.Book)
                .Where(bt => bt.Book.Title.Contains(bookName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Booktheme>> GetByPartialThemeNameAsync(string themeName)
        {
            return await _context.Bookthemes
                .Include(bt => bt.Theme)
                .Where(bt => bt.Theme.Name.Contains(themeName))
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetThemeNamesByBookIdAsync(int bookId)
        {
            return await _context.Bookthemes
                .Where(bt => bt.BookId == bookId)
                .Select(bt => bt.Theme.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetBookNamesByThemeIdAsync(int themeId)
        {
            return await _context.Bookthemes
                .Where(bt => bt.ThemeId == themeId)
                .Select(bt => bt.Book.Title)
                .ToListAsync();
        }
    }
}
