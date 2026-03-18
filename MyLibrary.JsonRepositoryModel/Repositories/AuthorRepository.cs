using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.JsonRepositoryModel.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(JsonDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Author>> GetAuthorsByBookAsync(int bookId)
    {
        var authorIds = _context.Bookauthors
            .Where(ba => ba.BookId == bookId)
            .Select(ba => ba.AuthorId)
            .ToList();

        return _context.Authors
            .Where(a => authorIds.Contains(a.Id))
            .ToList();
    }

    public async Task<IEnumerable<Author>> GetAuthorsByPartialNameAsync(string name)
    {
        return _context.Authors
            .Where(a => (a.First_Name + " " + a.Last_Name).Contains(name))
            .ToList();
    }

    public async Task<IEnumerable<Author>> GetAuthorsByPartialBioAsync(string bio)
    {
        return _context.Authors
            .Where(a => a.Bio != null && a.Bio.Contains(bio))
            .ToList();
    }
}
