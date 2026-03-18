using Microsoft.AspNetCore.Mvc;
using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.ApiModel.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: Users/GetAll
    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users);
    }

    // GET: Users/GetById&id=1
    [HttpGet("GetById")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // POST: Users/Create
    [HttpPost("Create")]
    public async Task<ActionResult<User>> Create([FromBody] User user)
    {
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // PUT: Users/Update
    [HttpPut("Update")]
    public async Task<IActionResult> Update(int id, [FromBody] User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.MaxConcurrentBorrows = user.MaxConcurrentBorrows;
        existingUser.CreatedAt = user.CreatedAt;

        await _userRepository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: Users/Delete&id=1
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
        return NoContent();
    }

    // GET: Users/GetByPartialName&name=John
    [HttpGet("GetByPartialName")]
    public async Task<ActionResult<IEnumerable<User>>> GetByPartialName(string name)
    {
        var users = await _userRepository.GetByPartialNameAsync(name);
        return Ok(users);
    }

    // GET: Users/GetByPartialEmail&email=example.com
    [HttpGet("GetByPartialEmail")]
    public async Task<ActionResult<IEnumerable<User>>> GetByPartialEmail(string email)
    {
        var users = await _userRepository.GetByPartialEmailAsync(email);
        return Ok(users);
    }

    // GET: Users/GetByMaxConcurrentBorrows&min=1&max=5
    [HttpGet("GetByMaxConcurrentBorrows")]
    public async Task<ActionResult<IEnumerable<User>>> GetByMaxConcurrentBorrows(int min, int max)
    {
        var users = await _userRepository.GetByMaxConcurrentBorrowsAsync(min, max);
        return Ok(users);
    }

    // GET: Users/GetUsersWithoutBorrowAllowance
    [HttpGet("GetUsersWithoutBorrowAllowance")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersWithoutBorrowAllowance()
    {
        var users = await _userRepository.GetUsersWithoutBorrowAllowanceAsync();
        return Ok(users);
    }

    // GET: Users/GetBorrowedBookNames&userId=1
    [HttpGet("GetBorrowedBookNames")]
    public async Task<ActionResult<IEnumerable<string>>> GetBorrowedBookNames(int userId)
    {
        var bookNames = await _userRepository.GetBorrowedBookNamesAsync(userId);
        return Ok(bookNames);
    }
}
