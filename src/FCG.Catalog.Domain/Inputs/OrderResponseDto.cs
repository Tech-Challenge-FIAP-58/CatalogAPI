namespace FCG.Catalog.Domain.Inputs
{
    public sealed record OrderResponseDto(
        int Id,
        DateTime OrderDate,
        int UserId,
        int GameId,
        decimal Price,
        string PaymentStatus,
        string CardName,
        string CardNumber,
        string ExpirationDate,
        string Cvv
    );
}
