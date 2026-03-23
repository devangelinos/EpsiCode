namespace EpsiCodeAPI.DTOs
{
    public record OrderSummaryDto(
        Guid Id,
        string Address,
        decimal TotalCost,
        DateTime CreationDate
    );

    public record CreateOrderDto(string Address);

    public record AddBookToOrderDto(
        Guid BookId,
        int Quantity
    );

    public record OrderBookDetailDto(
        Guid BookId,
        string Title,
        int Quantity,
        decimal PriceAtPurchase, 
        decimal SubTotal         
    );
}
