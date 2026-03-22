using EpsiCodeWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EpsiCodeWeb.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7261/api/books";

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            List<BookViewModel> books = new();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                books = JsonSerializer.Deserialize<List<BookViewModel>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View(books);
        }
    }
}
