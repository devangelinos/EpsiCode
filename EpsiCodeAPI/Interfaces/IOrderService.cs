using EpsiCodeAPI.DTOs;
using EpsiCodeAPI.Models;

namespace EpsiCodeAPI.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync();
        Task<Order> CreateOrderAsync(string address);
        Task<IEnumerable<OrderBookDetailDto>> GetOrderBooksAsync(Guid orderId);
        Task<(bool Success, string Message)> AddBookToOrderAsync(Guid orderId, Guid bookId, int quantity);
        Task<bool> RemoveBookFromOrderAsync(Guid orderId, Guid bookId);
        Task<bool> DeleteOrderAsync(Guid orderId);
    }
}
