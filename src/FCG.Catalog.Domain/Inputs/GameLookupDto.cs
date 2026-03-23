namespace FCG.Catalog.Domain.Inputs
{
    public record GameLookupDto(
        Guid Id,
        string Name,
        string Platform,
        string PublisherName,
        string Description,
        decimal Price,
        bool IsAvailable);
}
