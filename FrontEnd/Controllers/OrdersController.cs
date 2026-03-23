using EpsiCodeWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EpsiCodeWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7261/api/orders";
        private readonly string _booksApiUrl = "https://localhost:7261/api/books";

        public OrdersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderViewModel>>(content, options) ?? new();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string address)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(_apiBaseUrl, new { address });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var newId = doc.RootElement.GetProperty("id").GetGuid();

                return RedirectToAction("Details", new { id = newId });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Fetch Order Summary
            var orderResponse = await client.GetAsync(_apiBaseUrl);
            var allOrdersContent = await orderResponse.Content.ReadAsStringAsync();
            var allOrders = JsonSerializer.Deserialize<List<OrderViewModel>>(allOrdersContent, options);
            var currentOrder = allOrders?.FirstOrDefault(o => o.Id == id);

            if (currentOrder == null) return NotFound();

            // Fetch Books currently in the order
            var itemsResponse = await client.GetAsync($"{_apiBaseUrl}/{id}/books");
            var itemsContent = await itemsResponse.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderBookLineItemViewModel>>(itemsContent, options) ?? new();

            // Fetch ALL available books
            var booksResponse = await client.GetAsync(_booksApiUrl);
            var booksContent = await booksResponse.Content.ReadAsStringAsync();
            var availableBooks = JsonSerializer.Deserialize<List<BookViewModel>>(booksContent, options) ?? new();

            var viewModel = new OrderDetailViewModel
            {
                OrderId = id,
                Address = currentOrder.Address,
                TotalCost = currentOrder.TotalCost,
                OrderBooks = items,
                AvailableBooks = availableBooks
            };

            return View(viewModel);
        }

    }
}
