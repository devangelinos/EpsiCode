using EpsiCodeAPI.Data;
using EpsiCodeAPI.DTOs;
using EpsiCodeAPI.Interfaces;
using EpsiCodeAPI.Jobs;
using EpsiCodeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EpsiCodeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;
        private readonly BookService _bookService;

        public BooksController(IBookRepository repository, BookService bookService)
        {
            _repository = repository;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> FetchBooks()
        {
            await _bookService.SyncBooksAsync();
            return Ok(new { message = "Sync Complete" });
        }

        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] UpdatePriceDto dto)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return NotFound();

            book.Price = dto.NewPrice;
            await _repository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}/copies")]
        public async Task<IActionResult> UpdateCopies(Guid id, [FromBody] UpdateCopiesDto dto)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return NotFound();

            book.NumberOfCopies = dto.NewCount;
            await _repository.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("sync-status")]
        public IActionResult GetSyncStatus()
        {
            return Ok(new { lastRun = BookSyncJob.LastRunTime });
        }
    }
}
