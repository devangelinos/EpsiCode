namespace EpsiCodeAPI.DTOs
{

    public record UpdatePriceDto(decimal NewPrice);

    public record UpdateCopiesDto(int NewCount);

    public record BookResponseDto(
        Guid Id,
        string Title,
        string Author,
        string Cover,
        decimal Price,
        int NumberOfCopies
    );
}
