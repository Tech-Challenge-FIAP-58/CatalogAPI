namespace FCG.Core.Core.Inputs
{
    public sealed record GameResponseDto(
        Guid Id,
        string Name,
        string Platform,
        string PublisherName,
        string Description,
        double Price,
        DateTime CreatedAtUtc
        );
}
