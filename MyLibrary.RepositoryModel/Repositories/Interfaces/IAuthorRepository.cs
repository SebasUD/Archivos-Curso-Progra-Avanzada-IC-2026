
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<IEnumerable<Author>> GetAuthorsByBookAsync(int bookId);
        Task<IEnumerable<Author>> GetAuthorsByPartialNameAsync(string name);
        Task<IEnumerable<Author>> GetAuthorsByPartialBioAsync(string bio);
    }
}
