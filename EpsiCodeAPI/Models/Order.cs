using System.ComponentModel.DataAnnotations;

namespace EpsiCodeAPI.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Address { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public decimal TotalCost { get; set; }

        public List<OrderBook> OrderBooks { get; set; } = new();
    }
}
