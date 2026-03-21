using EpsiCodeAPI.Data;
using EpsiCodeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EpsiCodeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;
        private readonly AppDbContext _context;

        public BooksController(BookService bookService, AppDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            await _bookService.SyncBooksAsync();

            var books = await _context.Books.ToListAsync();
            return Ok(books);
        }
    }
}
