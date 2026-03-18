using Microsoft.AspNetCore.Mvc;
using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.ApiModel.Controllers;

[ApiController]
[Route("[controller]")]
public class BookAuthorsController : ControllerBase
{
    private readonly IBookAuthorRepository _bookAuthorRepository;

    public BookAuthorsController(IBookAuthorRepository bookAuthorRepository)
    {
        _bookAuthorRepository = bookAuthorRepository;
    }

    // GET: BookAuthors/GetAll
    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Bookauthor>>> GetAll()
    {
        var bookAuthors = await _bookAuthorRepository.GetAllAsync();
        return Ok(bookAuthors);
    }

    // GET: BookAuthors/GetById&id=1
    [HttpGet("GetById")]
    public async Task<ActionResult<Bookauthor>> GetById(int id)
    {
        var bookAuthor = await _bookAuthorRepository.GetByIdAsync(id);
        if (bookAuthor == null)
        {
            return NotFound();
        }
        return Ok(bookAuthor);
    }

    // POST: BookAuthors/Create
    [HttpPost("Create")]
    public async Task<ActionResult<Bookauthor>> Create([FromBody] Bookauthor bookAuthor)
    {
        await _bookAuthorRepository.AddAsync(bookAuthor);
        await _bookAuthorRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = bookAuthor.BookId }, bookAuthor);
    }

    // PUT: BookAuthors/Update
    [HttpPut("Update")]
    public async Task<IActionResult> Update(int bookId, int authorId, [FromBody] Bookauthor bookAuthor)
    {
        var bookAuthors = await _bookAuthorRepository.FindAsync(ba => ba.BookId == bookId && ba.AuthorId == authorId);
        var existingBookAuthor = bookAuthors.FirstOrDefault();
        
        if (existingBookAuthor == null)
        {
            return NotFound();
        }

        existingBookAuthor.Role = bookAuthor.Role;

        await _bookAuthorRepository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: BookAuthors/Delete?bookId=1&authorId=1
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int bookId, int authorId)
    {
        var bookAuthors = await _bookAuthorRepository.FindAsync(ba => ba.BookId == bookId && ba.AuthorId == authorId);
        var bookAuthor = bookAuthors.FirstOrDefault();
        
        if (bookAuthor == null)
        {
            return NotFound();
        }

        _bookAuthorRepository.Remove(bookAuthor);
        await _bookAuthorRepository.SaveChangesAsync();
        return NoContent();
    }
}
