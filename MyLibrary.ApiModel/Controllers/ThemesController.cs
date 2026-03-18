using Microsoft.AspNetCore.Mvc;
using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.ApiModel.Controllers;

[ApiController]
[Route("[controller]")]
public class ThemesController : ControllerBase
{
    private readonly IThemeRepository _themeRepository;

    public ThemesController(IThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
    }

    // GET: Themes/GetAll
    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Theme>>> GetAll()
    {
        var themes = await _themeRepository.GetAllAsync();
        return Ok(themes);
    }

    // GET: Themes/GetById&id=1
    [HttpGet("GetById")]
    public async Task<ActionResult<Theme>> GetById(int id)
    {
        var theme = await _themeRepository.GetByIdAsync(id);
        if (theme == null)
        {
            return NotFound();
        }
        return Ok(theme);
    }

    // POST: Themes/Create
    [HttpPost("Create")]
    public async Task<ActionResult<Theme>> Create([FromBody] Theme theme)
    {
        await _themeRepository.AddAsync(theme);
        await _themeRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = theme.Id }, theme);
    }

    // PUT: Themes/Update
    [HttpPut("Update")]
    public async Task<IActionResult> Update(int id, [FromBody] Theme theme)
    {
        var existingTheme = await _themeRepository.GetByIdAsync(id);
        if (existingTheme == null)
        {
            return NotFound();
        }

        existingTheme.Name = theme.Name;

        await _themeRepository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: Themes/Delete&id=1
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var theme = await _themeRepository.GetByIdAsync(id);
        if (theme == null)
        {
            return NotFound();
        }

        _themeRepository.Remove(theme);
        await _themeRepository.SaveChangesAsync();
        return NoContent();
    }
}
