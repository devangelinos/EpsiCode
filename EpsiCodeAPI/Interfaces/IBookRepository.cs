using EpsiCodeAPI.Models;

namespace EpsiCodeAPI.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(Guid id);
        Task<Book?> GetByNumberAsync(int number);
        Task AddRangeAsync(IEnumerable<Book> books);
        Task<bool> SaveChangesAsync();
    }
}
