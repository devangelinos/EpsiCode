using EpsiCodeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EpsiCodeAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderBook> OrderBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderBook>()
                .HasKey(ob => new { ob.OrderId, ob.BookId });

            modelBuilder.Entity<OrderBook>()
                .HasOne(ob => ob.Order)
                .WithMany(o => o.OrderBooks)
                .HasForeignKey(ob => ob.OrderId);

            modelBuilder.Entity<OrderBook>()
                .HasOne(ob => ob.Book)
                .WithMany()
                .HasForeignKey(ob => ob.BookId);
        }
    }
}
