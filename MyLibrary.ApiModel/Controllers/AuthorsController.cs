using Microsoft.AspNetCore.Mvc;
using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.ApiModel.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorRepository _authorRepository;
    private readonly JsonDbContext _jsonContext;

    public AuthorsController(IAuthorRepository authorRepository, JsonDbContext jsonContext)
    {
        _authorRepository = authorRepository;
        _jsonContext = jsonContext;
    }

    // GET: Authors/GetAll
    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Author>>> GetAll()
    {
        var authors = await _authorRepository.GetAllAsync();
        return Ok(authors);
    }

    // GET: Authors/GetById&id=1
    [HttpGet("GetById")]
    public async Task<ActionResult<Author>> GetById(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }
        return Ok(author);
    }

    // POST: Authors/Create
    [HttpPost("Create")]
    public async Task<ActionResult<Author>> Create([FromBody] Author author)
    {
        await _authorRepository.AddAsync(author);
        await _authorRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
    }

    // PUT: Authors/Update
    [HttpPut("Update")]
    public async Task<IActionResult> Update(int id, [FromBody] Author author)
    {
        var existingAuthor = await _authorRepository.GetByIdAsync(id);
        if (existingAuthor == null)
        {
            return NotFound();
        }

        existingAuthor.First_Name = author.First_Name;
        existingAuthor.Last_Name = author.Last_Name;
        existingAuthor.Bio = author.Bio;
        existingAuthor.CreatedAt = author.CreatedAt;

        await _authorRepository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: Authors/Delete&id=1
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        _authorRepository.Remove(author);
        await _authorRepository.SaveChangesAsync();
        return NoContent();
    }

    // GET: Authors/GetAuthorsByBook&bookId=1
    [HttpGet("GetAuthorsByBook")]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthorsByBook(int bookId)
    {
        var authors = await _authorRepository.GetAuthorsByBookAsync(bookId);
        return Ok(authors);
    }

    // GET: Authors/GetAuthorsByPartialName&name=John
    [HttpGet("GetAuthorsByPartialName")]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthorsByPartialName(string name)
    {
        var authors = await _authorRepository.GetAuthorsByPartialNameAsync(name);
        return Ok(authors);
    }

    // GET: Authors/GetAuthorsByPartialBio&bio=writer
    [HttpGet("GetAuthorsByPartialBio")]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthorsByPartialBio(string bio)
    {
        var authors = await _authorRepository.GetAuthorsByPartialBioAsync(bio);
        return Ok(authors);
    }
}
