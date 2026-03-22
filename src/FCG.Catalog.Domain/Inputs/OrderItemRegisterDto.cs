using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Domain.Inputs;

public sealed record class OrderItemRegisterDto
{
    [Required]
    public required Guid GameId { get; set; }
}
