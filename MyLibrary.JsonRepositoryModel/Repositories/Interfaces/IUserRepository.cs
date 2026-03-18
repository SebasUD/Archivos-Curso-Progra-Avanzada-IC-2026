using MyLibrary.DbModel.Entities;

namespace MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<IEnumerable<User>> GetByPartialNameAsync(string name);
    Task<IEnumerable<User>> GetByPartialEmailAsync(string email);
    Task<IEnumerable<User>> GetByMaxConcurrentBorrowsAsync(int min, int max);
    Task<IEnumerable<User>> GetUsersWithoutBorrowAllowanceAsync();
    Task<IEnumerable<string>> GetBorrowedBookNamesAsync(int userId);
}
