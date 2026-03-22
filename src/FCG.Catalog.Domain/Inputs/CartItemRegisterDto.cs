using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Domain.Inputs;

public sealed record class CartItemRegisterDto
{
    [Required]
    public Guid GameId { get; init; } = default!;

    [Required, MinLength(2)]
    public string Name { get; init; } = default!;

    [Required, MinLength(2)]
    public string Platform { get; init; } = default!;

    [Required, MinLength(2)]
    public string PublisherName { get; init; } = default!;

    [Required, MinLength(2), MaxLength(100)]
    public string Description { get; init; } = default!;

    [Required]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Preço deve ser maior que zero.")]
    public decimal UnitPrice { get; init; } = default!;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
    public int Quantity { get; init; } = default!;
}
