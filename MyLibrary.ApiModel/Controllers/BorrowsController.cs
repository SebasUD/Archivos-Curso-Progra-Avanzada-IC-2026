using Microsoft.AspNetCore.Mvc;
using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.ApiModel.Controllers;

[ApiController]
[Route("[controller]")]
public class BorrowsController : ControllerBase
{
    private readonly IBorrowRepository _borrowRepository;

    public BorrowsController(IBorrowRepository borrowRepository)
    {
        _borrowRepository = borrowRepository;
    }

    // GET: Borrows/GetAll
    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Borrow>>> GetAll()
    {
        var borrows = await _borrowRepository.GetAllAsync();
        return Ok(borrows);
    }

    // GET: Borrows/GetById&id=1
    [HttpGet("GetById")]
    public async Task<ActionResult<Borrow>> GetById(int id)
    {
        var borrow = await _borrowRepository.GetByIdAsync(id);
        if (borrow == null)
        {
            return NotFound();
        }
        return Ok(borrow);
    }

    // POST: Borrows/Create
    [HttpPost("Create")]
    public async Task<ActionResult<Borrow>> Create([FromBody] Borrow borrow)
    {
        await _borrowRepository.AddAsync(borrow);
        await _borrowRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = borrow.Id }, borrow);
    }

    // PUT: Borrows/Update
    [HttpPut("Update")]
    public async Task<IActionResult> Update(int id, [FromBody] Borrow borrow)
    {
        var existingBorrow = await _borrowRepository.GetByIdAsync(id);
        if (existingBorrow == null)
        {
            return NotFound();
        }

        existingBorrow.UserId = borrow.UserId;
        existingBorrow.BookCopyId = borrow.BookCopyId;
        existingBorrow.BorrowDate = borrow.BorrowDate;
        existingBorrow.DueDate = borrow.DueDate;
        existingBorrow.ReturnDate = borrow.ReturnDate;
        existingBorrow.Status = borrow.Status;
        existingBorrow.Notes = borrow.Notes;

        await _borrowRepository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: Borrows/Delete&id=1
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var borrow = await _borrowRepository.GetByIdAsync(id);
        if (borrow == null)
        {
            return NotFound();
        }

        _borrowRepository.Remove(borrow);
        await _borrowRepository.SaveChangesAsync();
        return NoContent();
    }
}
