
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories.Interfaces
{
    public interface IBookCopyRepository : IRepository<Bookcopy>
    {
        Task<IEnumerable<Bookcopy>> GetByStatusAsync(int status);
        Task<IEnumerable<Bookcopy>> GetByIsbnAsync(string isbn);
        Task<IEnumerable<Bookcopy>> GetByBookPropertiesAsync(string title, string description, string publisher);
    }
}
