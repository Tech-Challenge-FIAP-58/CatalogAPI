namespace FCG.Catalog.Domain.Inputs
{
    public sealed record GameLibraryGameResponseDto(
        Guid GameId,
        string Name,
        string Platform,
        string PublisherName,
        string Description,
        decimal UnitPrice
    );
}
