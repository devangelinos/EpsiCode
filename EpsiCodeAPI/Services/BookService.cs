using EpsiCodeAPI.Data;
using EpsiCodeAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EpsiCodeAPI.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private const string ApiUrl = "https://potterapi-fedeperin.vercel.app/en/books";

        public BookService(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task SyncBooksAsync()
        {
            var response = await _httpClient.GetAsync(ApiUrl);
            if (!response.IsSuccessStatusCode) return;

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiBooks = JsonSerializer.Deserialize<List<Book>>(content, options) ?? new List<Book>();

            var existingBookNumbers = await _context.Books
                .Select(b => b.Number)
                .ToListAsync();

            var newBooks = apiBooks.Where(apiBook => !existingBookNumbers.Contains(apiBook.Number)).ToList();

            if (newBooks.Any())
            {
                foreach (var book in newBooks)
                {
                    book.Id = Guid.NewGuid();
                    book.Price = 29.99m;
                    book.NumberOfCopies = 10;
                }

                _context.Books.AddRange(newBooks);
                await _context.SaveChangesAsync();
            }
        }
    }
}
