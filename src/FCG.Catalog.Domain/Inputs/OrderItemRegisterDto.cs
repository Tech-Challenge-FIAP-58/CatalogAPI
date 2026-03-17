namespace FCG.Catalog.Domain.Inputs;

public class OrderItemRegisterDto
{
    public required Guid GameId { get; set; }
    public required string GameName { get; set; }

}
