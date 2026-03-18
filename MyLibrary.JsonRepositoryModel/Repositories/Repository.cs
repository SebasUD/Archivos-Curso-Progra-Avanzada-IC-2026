using System.Linq.Expressions;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.JsonRepositoryModel.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly JsonDbContext _context;
    protected readonly List<T> _data;

    public Repository(JsonDbContext context)
    {
        _context = context;
        _data = GetDbSet();
    }

    protected virtual List<T> GetDbSet()
    {
        var entityType = typeof(T).Name;
        return entityType switch
        {
            "Book" => _context.Books.Cast<T>().ToList(),
            "Author" => _context.Authors.Cast<T>().ToList(),
            "User" => _context.Users.Cast<T>().ToList(),
            "Theme" => _context.Themes.Cast<T>().ToList(),
            "Bookauthor" => _context.Bookauthors.Cast<T>().ToList(),
            "Bookcopy" => _context.Bookcopies.Cast<T>().ToList(),
            "Booktheme" => _context.Bookthemes.Cast<T>().ToList(),
            "Borrow" => _context.Borrows.Cast<T>().ToList(),
            _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
        };
    }

    public virtual Task<T?> GetByIdAsync(int id)
    {
        var entity = _data.FirstOrDefault(e => GetIdProperty(e) == id);
        return Task.FromResult(entity);
    }

    public virtual Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_data.ToList());
    }

    public virtual Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        var compiled = predicate.Compile();
        var result = _data.Where(compiled).ToList();
        return Task.FromResult<IEnumerable<T>>(result);
    }

    public virtual Task AddAsync(T entity)
    {
        _data.Add(entity);
        return Task.CompletedTask;
    }

    public virtual Task AddRangeAsync(IEnumerable<T> entities)
    {
        _data.AddRange(entities);
        return Task.CompletedTask;
    }

    public virtual void Remove(T entity)
    {
        _data.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        var entityList = entities.ToList();
        foreach (var entity in entityList)
        {
            _data.Remove(entity);
        }
    }

    public virtual Task<int> SaveChangesAsync()
    {
        _context.SaveChanges();
        return Task.FromResult(_data.Count);
    }

    protected int GetIdProperty(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
        {
            // Handle composite keys
            if (typeof(T).Name == "Bookauthor")
            {
                var bookId = (int)(typeof(T).GetProperty("BookId")?.GetValue(entity) ?? 0);
                var authorId = (int)(typeof(T).GetProperty("AuthorId")?.GetValue(entity) ?? 0);
                return bookId * 1000 + authorId;
            }
            if (typeof(T).Name == "Booktheme")
            {
                var bookId = (int)(typeof(T).GetProperty("BookId")?.GetValue(entity) ?? 0);
                var themeId = (int)(typeof(T).GetProperty("ThemeId")?.GetValue(entity) ?? 0);
                return bookId * 1000 + themeId;
            }
            return 0;
        }
        return (int)(idProperty.GetValue(entity) ?? 0);
    }
}
