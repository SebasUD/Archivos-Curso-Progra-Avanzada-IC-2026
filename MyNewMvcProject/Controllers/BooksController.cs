using Microsoft.AspNetCore.Mvc;
using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyNewMvcProject.Controllers;

public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    // GET: Books/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Isbn,Title,Description,Publisher,PublishedDate,Pages,CreatedAt")] Book book)
    {
        if (ModelState.IsValid)
        {
            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: Books
    public async Task<IActionResult> Index(string title = "", string description = "", string publisher = "")
    {
        var books = await _bookRepository.FindBooksAsync(title, description, publisher);
        ViewData["SearchTitle"] = title;
        ViewData["SearchDescription"] = description;
        ViewData["SearchPublisher"] = publisher;
        return View(books);
    }

    // GET: Books/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    // GET: Books/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Isbn,Title,Description,Publisher,PublishedDate,Pages,CreatedAt")] Book book)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
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
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: Books/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    // POST: Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book != null)
        {
            _bookRepository.Remove(book);
            await _bookRepository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}