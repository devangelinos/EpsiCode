using EpsiCodeAPI.Data;
using EpsiCodeAPI.Interfaces;
using EpsiCodeAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EpsiCodeAPI.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;
        private readonly IBookRepository _repository;
        private readonly ILogger<BookService> _logger;
        private const string ApiUrl = "https://potterapi-fedeperin.vercel.app/en/books";

        public BookService(HttpClient httpClient, IBookRepository repository, ILogger<BookService> logger)
        {
            _httpClient = httpClient;
            _repository = repository;
            _logger = logger;
        }

        public async Task SyncBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var apiBooks = JsonSerializer.Deserialize<List<Book>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                var newBooks = new List<Book>();
                foreach (var apiBook in apiBooks)
                {
                    var existingBook = await _repository.GetByNumberAsync(apiBook.Number);

                    if (existingBook == null)
                    {
                        apiBook.Id = Guid.NewGuid();
                        apiBook.Price = 29.99m;
                        apiBook.NumberOfCopies = 5;
                        newBooks.Add(apiBook);
                    }
                }

                if (newBooks.Any())
                {
                    await _repository.AddRangeAsync(newBooks);
                    await _repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while syncing books from external API.");
                throw;
            }
        }
    }
}
