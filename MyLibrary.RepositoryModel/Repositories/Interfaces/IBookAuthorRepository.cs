
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories.Interfaces
{
    public interface IBookAuthorRepository : IRepository<Bookauthor>
    {
        Task<Bookauthor> GetByIdAsync(int bookId, int authorId);
        Task<IEnumerable<string>> GetAuthorRolesByBookAsync(int bookId);
    }
}
