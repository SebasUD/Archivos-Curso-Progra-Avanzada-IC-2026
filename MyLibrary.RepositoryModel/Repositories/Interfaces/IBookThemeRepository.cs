
using MyLibrary.DbModel.Entities;
using MyLibrary.RepositoryModel.Repositories.Interfaces;

namespace MyLibrary.RepositoryModel.Repositories.Interfaces
{
    public interface IBookThemeRepository : IRepository<Booktheme>
    {
        Task<IEnumerable<Booktheme>> GetByPartialBookNameAsync(string bookName);
        Task<IEnumerable<Booktheme>> GetByPartialThemeNameAsync(string themeName);
        Task<IEnumerable<string>> GetThemeNamesByBookIdAsync(int bookId);
        Task<IEnumerable<string>> GetBookNamesByThemeIdAsync(int themeId);
    }
}
