using MyLibrary.DbModel.Entities;

namespace MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId);
    Task<IEnumerable<Book>> GetBooksByThemeAsync(int themeId);
    Task<IEnumerable<Book>> FindBooksAsync(string title, string description, string publisher);
    Task<Book?> GetBookByIsbnAsync(string isbn);
}
