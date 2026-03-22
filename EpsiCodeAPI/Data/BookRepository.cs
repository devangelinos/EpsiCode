using EpsiCodeAPI.Interfaces;
using EpsiCodeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EpsiCodeAPI.Data
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Book>> GetAllAsync() => await _context.Books.ToListAsync();

        public async Task<Book?> GetByIdAsync(Guid id) => await _context.Books.FindAsync(id);

        public async Task<Book?> GetByNumberAsync(int number) =>
            await _context.Books.FirstOrDefaultAsync(b => b.Number == number);

        public async Task AddRangeAsync(IEnumerable<Book> books) => await _context.Books.AddRangeAsync(books);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}
