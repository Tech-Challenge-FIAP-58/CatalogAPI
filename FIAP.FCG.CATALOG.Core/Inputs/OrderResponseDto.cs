namespace FIAP.FCG.CATALOG.Core.Inputs
{
    public sealed record OrderResponseDto(
        int Id,
        DateTime OrderDate,
        int UserId,
        int GameId,
        Decimal Price,
        string PaymentStatus,
        string CardName,
        string CardNumber,
        string ExpirationDate,
        string Cvv
    );
}
