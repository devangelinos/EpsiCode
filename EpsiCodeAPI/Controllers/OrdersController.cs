using EpsiCodeAPI.DTOs;
using EpsiCodeAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpsiCodeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var order = await _orderService.CreateOrderAsync(dto.Address);
            return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
        }

        [HttpGet("{id}/books")]
        public async Task<IActionResult> GetOrderBooks(Guid id)
        {
            var books = await _orderService.GetOrderBooksAsync(id);
            return Ok(books);
        }

        [HttpPost("{id}/books")]
        public async Task<IActionResult> AddBookToOrder(Guid id, [FromBody] AddBookToOrderDto dto)
        {
            var result = await _orderService.AddBookToOrderAsync(id, dto.BookId, dto.Quantity);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}/books/{bookId}")]
        public async Task<IActionResult> RemoveBook(Guid id, Guid bookId)
        {
            var success = await _orderService.RemoveBookFromOrderAsync(id, bookId);
            if (!success) return NotFound(new { message = "Order or Book not found in this order." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var success = await _orderService.DeleteOrderAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
