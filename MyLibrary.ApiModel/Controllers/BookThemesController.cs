using Microsoft.AspNetCore.Mvc;
using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.ApiModel.Controllers;

[ApiController]
[Route("[controller]")]
public class BookThemesController : ControllerBase
{
    private readonly IBookThemeRepository _bookThemeRepository;

    public BookThemesController(IBookThemeRepository bookThemeRepository)
    {
        _bookThemeRepository = bookThemeRepository;
    }

    // GET: BookThemes/GetAll
    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Booktheme>>> GetAll()
    {
        var bookThemes = await _bookThemeRepository.GetAllAsync();
        return Ok(bookThemes);
    }

    // GET: BookThemes/GetById&id=1
    [HttpGet("GetById")]
    public async Task<ActionResult<Booktheme>> GetById(int id)
    {
        var bookTheme = await _bookThemeRepository.GetByIdAsync(id);
        if (bookTheme == null)
        {
            return NotFound();
        }
        return Ok(bookTheme);
    }

    // POST: BookThemes/Create
    [HttpPost("Create")]
    public async Task<ActionResult<Booktheme>> Create([FromBody] Booktheme bookTheme)
    {
        await _bookThemeRepository.AddAsync(bookTheme);
        await _bookThemeRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = bookTheme.BookId }, bookTheme);
    }

    // PUT: BookThemes/Update
    [HttpPut("Update")]
    public async Task<IActionResult> Update(int bookId, int themeId, [FromBody] Booktheme bookTheme)
    {
        var bookThemes = await _bookThemeRepository.FindAsync(bt => bt.BookId == bookId && bt.ThemeId == themeId);
        var existingBookTheme = bookThemes.FirstOrDefault();
        
        if (existingBookTheme == null)
        {
            return NotFound();
        }

        // Booktheme has no other properties to update besides the keys

        await _bookThemeRepository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: BookThemes/Delete?bookId=1&themeId=1
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int bookId, int themeId)
    {
        var bookThemes = await _bookThemeRepository.FindAsync(bt => bt.BookId == bookId && bt.ThemeId == themeId);
        var bookTheme = bookThemes.FirstOrDefault();
        
        if (bookTheme == null)
        {
            return NotFound();
        }

        _bookThemeRepository.Remove(bookTheme);
        await _bookThemeRepository.SaveChangesAsync();
        return NoContent();
    }
}
