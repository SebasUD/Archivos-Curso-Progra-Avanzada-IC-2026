using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.JsonRepositoryModel.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(JsonDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<User>> GetByPartialNameAsync(string name)
    {
        return _context.Users
            .Where(u => u.Name.Contains(name))
            .ToList();
    }

    public async Task<IEnumerable<User>> GetByPartialEmailAsync(string email)
    {
        return _context.Users
            .Where(u => u.Email != null && u.Email.Contains(email))
            .ToList();
    }

    public async Task<IEnumerable<User>> GetByMaxConcurrentBorrowsAsync(int min, int max)
    {
        return _context.Users
            .Where(u => u.MaxConcurrentBorrows >= min && u.MaxConcurrentBorrows <= max)
            .ToList();
    }

    public async Task<IEnumerable<User>> GetUsersWithoutBorrowAllowanceAsync()
    {
        return _context.Users
            .Where(u => u.MaxConcurrentBorrows == 0)
            .ToList();
    }

    public async Task<IEnumerable<string>> GetBorrowedBookNamesAsync(int userId)
    {
        return _context.Borrows
            .Where(b => b.UserId == userId)
            .Select(b => _context.Bookcopies
                .Where(bc => bc.Id == b.BookCopyId)
                .Select(bc => _context.Books
                    .Where(book => book.Id == bc.BookId)
                    .Select(book => book.Title)
                    .FirstOrDefault())
                .FirstOrDefault())
            .Distinct()
            .Where(title => title != null)
            .Cast<string>()
            .ToList();
    }
}
