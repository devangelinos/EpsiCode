namespace EpsiCodeWeb.Models
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
