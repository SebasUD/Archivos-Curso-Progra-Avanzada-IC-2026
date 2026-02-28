
using Microsoft.EntityFrameworkCore;
using MyLibrary.DbModel.Context;
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        public AuthorRepository(MySqlDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Author>> GetAuthorsByBookAsync(int bookId)
        {
            return await _context.Bookauthors
                .Where(ba => ba.BookId == bookId)
                .Select(ba => ba.Author)
                .ToListAsync();
        }

        public async Task<IEnumerable<Author>> GetAuthorsByPartialNameAsync(string name)
        {
            return await _context.Authors
                .Where(a => (a.FirstName + " " + a.LastName).Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Author>> GetAuthorsByPartialBioAsync(string bio)
        {
            return await _context.Authors
                .Where(a => a.Bio.Contains(bio))
                .ToListAsync();
        }
    }
}
