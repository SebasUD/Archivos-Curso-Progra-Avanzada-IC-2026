using Microsoft.AspNetCore.Mvc;
using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.ApiModel.Controllers;

[ApiController]
[Route("[controller]")]
public class BookCopiesController : ControllerBase
{
    private readonly IBookCopyRepository _bookCopyRepository;

    public BookCopiesController(IBookCopyRepository bookCopyRepository)
    {
        _bookCopyRepository = bookCopyRepository;
    }

    // GET: BookCopies/GetAll
    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Bookcopy>>> GetAll()
    {
        var bookCopies = await _bookCopyRepository.GetAllAsync();
        return Ok(bookCopies);
    }

    // GET: BookCopies/GetById&id=1
    [HttpGet("GetById")]
    public async Task<ActionResult<Bookcopy>> GetById(int id)
    {
        var bookCopy = await _bookCopyRepository.GetByIdAsync(id);
        if (bookCopy == null)
        {
            return NotFound();
        }
        return Ok(bookCopy);
    }

    // POST: BookCopies/Create
    [HttpPost("Create")]
    public async Task<ActionResult<Bookcopy>> Create([FromBody] Bookcopy bookCopy)
    {
        await _bookCopyRepository.AddAsync(bookCopy);
        await _bookCopyRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = bookCopy.Id }, bookCopy);
    }

    // PUT: BookCopies/Update
    [HttpPut("Update")]
    public async Task<IActionResult> Update(int id, [FromBody] Bookcopy bookCopy)
    {
        var existingBookCopy = await _bookCopyRepository.GetByIdAsync(id);
        if (existingBookCopy == null)
        {
            return NotFound();
        }

        existingBookCopy.BookId = bookCopy.BookId;
        existingBookCopy.Barcode = bookCopy.Barcode;
        existingBookCopy.Status = bookCopy.Status;
        existingBookCopy.PenaltyWeight = bookCopy.PenaltyWeight;
        existingBookCopy.AddedAt = bookCopy.AddedAt;

        await _bookCopyRepository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: BookCopies/Delete&id=1
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var bookCopy = await _bookCopyRepository.GetByIdAsync(id);
        if (bookCopy == null)
        {
            return NotFound();
        }

        _bookCopyRepository.Remove(bookCopy);
        await _bookCopyRepository.SaveChangesAsync();
        return NoContent();
    }
}
