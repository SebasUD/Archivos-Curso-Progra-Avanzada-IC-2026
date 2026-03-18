
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetByPartialNameAsync(string name);
        Task<IEnumerable<User>> GetByPartialEmailAsync(string email);
        Task<IEnumerable<User>> GetByMaxConcurrentBorrowsAsync(int min, int max);
        Task<IEnumerable<User>> GetUsersWithoutBorrowAllowanceAsync();
        Task<IEnumerable<string>> GetBorrowedBookNamesAsync(int userId);
    }
}
