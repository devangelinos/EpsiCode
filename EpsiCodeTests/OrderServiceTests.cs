using EpsiCodeAPI.Data;
using EpsiCodeAPI.Models;
using EpsiCodeAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace EpsiCodeTests
{
    public class OrderServiceTests
    {
        private AppDbContext GetDatabase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        #region CreateOrder Tests
        [Fact]
        public async Task CreateOrder_HappyPath_ReturnsNewOrder()
        {
            var context = GetDatabase();
            var service = new OrderService(context);

            var result = await service.CreateOrderAsync("Test Address 123");

            Assert.NotNull(result);
            Assert.Equal("Test Address 123", result.Address);
            Assert.Equal(0, result.TotalCost);
        }
        #endregion

        #region AddBookToOrder Tests
        [Fact]
        public async Task AddBookToOrder_HappyPath_UpdatesStockAndTotal()
        {
            var context = GetDatabase();
            var service = new OrderService(context);
            var bookId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            context.Books.Add(new Book { Id = bookId, NumberOfCopies = 10, Price = 20 });

            context.Orders.Add(new Order
            {
                Id = orderId,
                Address = "Test Address 123",
                CreationDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var result = await service.AddBookToOrderAsync(orderId, bookId, 2);

            var updatedBook = await context.Books.FindAsync(bookId);
            var updatedOrder = await context.Orders.FindAsync(orderId);

            Assert.True(result.Success);
            Assert.Equal(8, updatedBook!.NumberOfCopies);
            Assert.Equal(40, updatedOrder!.TotalCost);
        }

        [Fact]
        public async Task AddBookToOrder_UnhappyPath_InsufficientStock_ReturnsFalse()
        {
            var context = GetDatabase();
            var service = new OrderService(context);
            var bookId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            context.Books.Add(new Book { Id = bookId, NumberOfCopies = 1, Price = 10 });
            
            context.Orders.Add(new Order
            {
                Id = orderId,
                Address = "Test Address 123",
                CreationDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var result = await service.AddBookToOrderAsync(orderId, bookId, 5);

            Assert.False(result.Success);
            Assert.Contains("Not enough stock", result.Message);
        }
        #endregion

        #region RemoveBook Tests
        [Fact]
        public async Task RemoveBook_HappyPath_RestoresStockAndTotal()
        {
            var context = GetDatabase();
            var service = new OrderService(context);
            var bookId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            context.Books.Add(new Book { Id = bookId, NumberOfCopies = 5, Price = 10 });
            var order = new Order { 
                Id = orderId, 
                TotalCost = 20,
                Address = "Test Address 123"            
            };
            order.OrderBooks.Add(new OrderBook { OrderId = orderId, BookId = bookId, Quantity = 2, PriceAtPurchase = 10 });
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var result = await service.RemoveBookFromOrderAsync(orderId, bookId);

            var updatedBook = await context.Books.FindAsync(bookId);
            var updatedOrder = await context.Orders.FindAsync(orderId);

            Assert.True(result);
            Assert.Equal(7, updatedBook!.NumberOfCopies);
            Assert.Equal(0, updatedOrder!.TotalCost);
        }

        [Fact]
        public async Task RemoveBook_UnhappyPath_ItemNotFound_ReturnsFalse()
        {
            var context = GetDatabase();
            var service = new OrderService(context);

            var result = await service.RemoveBookFromOrderAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.False(result);
        }
        #endregion

        #region DeleteOrder Tests
        [Fact]
        public async Task DeleteOrder_HappyPath_RemovesFromDb()
        {
            var context = GetDatabase();
            var service = new OrderService(context);

            var orderId = Guid.NewGuid();
            context.Orders.Add(new Order
            {
                Id = orderId,
                Address = "Test Address 123",
                CreationDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var result = await service.DeleteOrderAsync(orderId);

            var deletedOrder = await context.Orders.FindAsync(orderId);
            Assert.True(result);
            Assert.Null(deletedOrder);
        }

        [Fact]
        public async Task DeleteOrder_UnhappyPath_NonExistent_ReturnsFalse()
        {
            var context = GetDatabase();
            var service = new OrderService(context);

            var result = await service.DeleteOrderAsync(Guid.NewGuid());

            Assert.False(result);
        }
        #endregion
    }
}
