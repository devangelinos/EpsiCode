using EpsiCodeAPI.Data;
using EpsiCodeAPI.DTOs;
using EpsiCodeAPI.Interfaces;
using EpsiCodeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EpsiCodeAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(string address)
        {
            var order = new Order { Id = Guid.NewGuid(), Address = address, CreationDate = DateTime.UtcNow, TotalCost = 0 };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<(bool Success, string Message)> AddBookToOrderAsync(Guid orderId, Guid bookId, int quantity)
        {
            var order = await _context.Orders.Include(o => o.OrderBooks).FirstOrDefaultAsync(o => o.Id == orderId);
            var book = await _context.Books.FindAsync(bookId);

            //Validations
            if (order == null) return (false, "Order not found.");
            if (book == null) return (false, "Book not found.");
            if (book.NumberOfCopies <= 0) return (false, "Book is out of stock.");
            if (book.NumberOfCopies < quantity) return (false, $"Not enough stock. Available: {book.NumberOfCopies}");

            var existingItem = order.OrderBooks.FirstOrDefault(ob => ob.BookId == bookId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                order.OrderBooks.Add(new OrderBook
                {
                    OrderId = orderId,
                    BookId = bookId,
                    Quantity = quantity,
                    PriceAtPurchase = book.Price
                });
            }

            book.NumberOfCopies -= quantity;
            order.TotalCost += (book.Price * quantity);

            await _context.SaveChangesAsync();
            return (true, "Book added successfully.");
        }

        public async Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Select(o => new OrderSummaryDto(o.Id, o.Address, o.TotalCost, o.CreationDate))
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderBookDetailDto>> GetOrderBooksAsync(Guid orderId)
        {
            return await _context.OrderBooks
                .Where(ob => ob.OrderId == orderId)
                .Select(ob => new OrderBookDetailDto(
                    ob.BookId,
                    ob.Book.Title,
                    ob.Quantity,
                    ob.PriceAtPurchase,
                    ob.Quantity * ob.PriceAtPurchase))
                .ToListAsync();
        }

        public async Task<bool> RemoveBookFromOrderAsync(Guid orderId, Guid bookId)
        {
            var order = await _context.Orders.Include(o => o.OrderBooks).FirstOrDefaultAsync(o => o.Id == orderId);
            var itemToRemove = order?.OrderBooks.FirstOrDefault(ob => ob.BookId == bookId);

            if (order == null || itemToRemove == null) return false;

            var book = await _context.Books.FindAsync(bookId);
            if (book != null)
            {
                book.NumberOfCopies += itemToRemove.Quantity;
                order.TotalCost -= (itemToRemove.PriceAtPurchase * itemToRemove.Quantity);
            }

            _context.OrderBooks.Remove(itemToRemove);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrderAsync(Guid orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderBooks).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return false;

            foreach (var item in order.OrderBooks)
            {
                var book = await _context.Books.FindAsync(item.BookId);
                if (book != null) book.NumberOfCopies += item.Quantity;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
