
using Microsoft.EntityFrameworkCore;
using MyLibrary.DbModel.Context;
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories
{
    public class BookAuthorRepository : Repository<Bookauthor>, IBookAuthorRepository
    {
        public BookAuthorRepository(MySqlDbContext context) : base(context)
        {
        }

        public async Task<Bookauthor> GetByIdAsync(int bookId, int authorId)
        {
            return await _context.Bookauthors
                .FirstOrDefaultAsync(ba => ba.BookId == bookId && ba.AuthorId == authorId);
        }

        public async Task<IEnumerable<string>> GetAuthorRolesByBookAsync(int bookId)
        {
            return await _context.Bookauthors
                .Where(ba => ba.BookId == bookId)
                .Select(ba => ba.Author.FirstName + " " + ba.Author.LastName + " - " + ba.Role)
                .ToListAsync();
        }
    }
}
