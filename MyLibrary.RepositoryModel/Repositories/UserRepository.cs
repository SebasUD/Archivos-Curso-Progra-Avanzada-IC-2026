
using Microsoft.EntityFrameworkCore;
using MyLibrary.DbModel.Context;
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MySqlDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> GetByPartialNameAsync(string name)
        {
            return await _context.Users
                .Where(u => u.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByPartialEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email.Contains(email))
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByMaxConcurrentBorrowsAsync(int min, int max)
        {
            return await _context.Users
                .Where(u => u.MaxConcurrentBorrows >= min && u.MaxConcurrentBorrows <= max)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithoutBorrowAllowanceAsync()
        {
            return await _context.Users
                .Where(u => u.MaxConcurrentBorrows == 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetBorrowedBookNamesAsync(int userId)
        {
            return await _context.Borrows
                .Where(b => b.UserId == userId)
                .Select(b => b.BookCopy.Book.Title)
                .Distinct()
                .ToListAsync();
        }
    }
}
