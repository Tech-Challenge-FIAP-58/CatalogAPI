namespace FCG.Core.Core.Inputs
{
    public sealed record OrderResponseDto(
        Guid Id,
        DateTime OrderDate,
        int UserId,
        Guid GameId,
        decimal Price,
        string PaymentStatus,
        string CardName,
        string CardNumber,
        string ExpirationDate,
        string Cvv
    );
}
