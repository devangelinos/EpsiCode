namespace EpsiCodeWeb.Models
{
    public class OrderDetailViewModel
    {
        public Guid OrderId { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }

        public List<OrderBookLineItemViewModel> OrderBooks { get; set; } = new();

        public List<BookViewModel> AvailableBooks { get; set; } = new();
    }

    public class OrderBookLineItemViewModel
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public decimal SubTotal { get; set; }
    }
}
