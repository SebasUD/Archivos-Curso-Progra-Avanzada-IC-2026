using Microsoft.AspNetCore.Mvc;
using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.ApiModel.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    // GET: Books/GetAll
    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll()
    {
        var books = await _bookRepository.GetAllAsync();
        return Ok(books);
    }

    // GET: Books/GetById&id=1
    [HttpGet("GetById")]
    public async Task<ActionResult<Book>> GetById(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return Ok(book);
    }

    // POST: Books/Create
    [HttpPost("Create")]
    public async Task<ActionResult<Book>> Create([FromBody] Book book)
    {
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    // PUT: Books/Update
    [HttpPut("Update")]
    public async Task<IActionResult> Update(int id, [FromBody] Book book)
    {
        var existingBook = await _bookRepository.GetByIdAsync(id);
        if (existingBook == null)
        {
            return NotFound();
        }

        existingBook.Isbn = book.Isbn;
        existingBook.Title = book.Title;
        existingBook.Description = book.Description;
        existingBook.Publisher = book.Publisher;
        existingBook.PublishedDate = book.PublishedDate;
        existingBook.Pages = book.Pages;
        existingBook.CreatedAt = book.CreatedAt;

        await _bookRepository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: Books/Delete&id=1
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _bookRepository.Remove(book);
        await _bookRepository.SaveChangesAsync();
        return NoContent();
    }

    // GET: Books/GetBooksByAuthor&authorId=1
    [HttpGet("GetBooksByAuthor")]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(int authorId)
    {
        var books = await _bookRepository.GetBooksByAuthorAsync(authorId);
        return Ok(books);
    }

    // GET: Books/GetBooksByTheme&themeId=1
    [HttpGet("GetBooksByTheme")]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooksByTheme(int themeId)
    {
        var books = await _bookRepository.GetBooksByThemeAsync(themeId);
        return Ok(books);
    }

    // GET: Books/FindBooks?title=...&description=...&publisher=...
    [HttpGet("FindBooks")]
    public async Task<ActionResult<IEnumerable<Book>>> FindBooks(
        [FromQuery] string title = "",
        [FromQuery] string description = "",
        [FromQuery] string publisher = "")
    {
        var books = await _bookRepository.FindBooksAsync(title, description, publisher);
        return Ok(books);
    }

    // GET: Books/GetBookByIsbn&isbn=978-0-13-468599-1
    [HttpGet("GetBookByIsbn")]
    public async Task<ActionResult<Book>> GetBookByIsbn(string isbn)
    {
        var book = await _bookRepository.GetBookByIsbnAsync(isbn);
        if (book == null)
        {
            return NotFound();
        }
        return Ok(book);
    }
}
