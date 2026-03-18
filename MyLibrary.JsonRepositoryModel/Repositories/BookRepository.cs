using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.JsonRepositoryModel.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(JsonDbContext context) : base(context)
    {
    }

    public override async Task AddAsync(Book entity)
    {
        if (entity.Id == 0)
        {
            entity.Id = _context.Books.Any() ? _context.Books.Max(b => b.Id) + 1 : 1;
        }
        entity.CreatedAt = entity.CreatedAt ?? DateTime.Now;
        await base.AddAsync(entity);
    }

    public override async Task<Book?> GetByIdAsync(int id)
    {
        var book = await base.GetByIdAsync(id);
        if (book != null)
        {
            PopulateNavigationProperties(book);
        }
        return book;
    }

    public override async Task<IEnumerable<Book>> GetAllAsync()
    {
        var books = await base.GetAllAsync();
        foreach (var book in books)
        {
            PopulateNavigationProperties(book);
        }
        return books;
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
    {
        var bookIds = _context.Bookauthors
            .Where(ba => ba.AuthorId == authorId)
            .Select(ba => ba.BookId)
            .ToList();

        var books = _context.Books
            .Where(b => bookIds.Contains(b.Id))
            .ToList();
        foreach (var book in books)
        {
            PopulateNavigationProperties(book);
        }
        return books;
    }

    public async Task<IEnumerable<Book>> GetBooksByThemeAsync(int themeId)
    {
        var bookIds = _context.Bookthemes
            .Where(bt => bt.ThemeId == themeId)
            .Select(bt => bt.BookId)
            .ToList();

        var books = _context.Books
            .Where(b => bookIds.Contains(b.Id))
            .ToList();
        foreach (var book in books)
        {
            PopulateNavigationProperties(book);
        }
        return books;
    }

    public async Task<IEnumerable<Book>> FindBooksAsync(string title, string description, string publisher)
    {
        var books = _context.Books
            .Where(b => (string.IsNullOrEmpty(title) || (b.Title != null && b.Title.Contains(title, StringComparison.OrdinalIgnoreCase))) &&
                        (string.IsNullOrEmpty(description) || (b.Description != null && b.Description.Contains(description, StringComparison.OrdinalIgnoreCase))) &&
                        (string.IsNullOrEmpty(publisher) || (b.Publisher != null && b.Publisher.Contains(publisher, StringComparison.OrdinalIgnoreCase))))
            .ToList();
        foreach (var book in books)
        {
            PopulateNavigationProperties(book);
        }
        return books;
    }

    public async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        var book = _context.Books
            .FirstOrDefault(b => b.Isbn == isbn);
        if (book != null)
        {
            PopulateNavigationProperties(book);
        }
        return book;
    }

    private void PopulateNavigationProperties(Book book)
    {
        book.Bookauthors = _context.Bookauthors.Where(ba => ba.BookId == book.Id).ToList();
        foreach (var ba in book.Bookauthors)
        {
            ba.Author = _context.Authors.FirstOrDefault(a => a.Id == ba.AuthorId);
            ba.Book = null; // avoid cycle
        }

        book.Bookcopies = _context.Bookcopies.Where(bc => bc.BookId == book.Id).ToList();
        foreach (var bc in book.Bookcopies)
        {
            bc.Book = null; // avoid cycle
        }

        book.Bookthemes = _context.Bookthemes.Where(bt => bt.BookId == book.Id).ToList();
        foreach (var bt in book.Bookthemes)
        {
            bt.Theme = _context.Themes.FirstOrDefault(t => t.Id == bt.ThemeId);
            bt.Book = null; // avoid cycle
        }
    }
}
