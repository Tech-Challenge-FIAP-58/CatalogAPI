using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Domain.Inputs;

public sealed record class CartAddItemDto
{
    [Required]
    public int UserId { get; init; } = default!;

    [Required]
    public Guid GameId { get; init; } = default!;
}
