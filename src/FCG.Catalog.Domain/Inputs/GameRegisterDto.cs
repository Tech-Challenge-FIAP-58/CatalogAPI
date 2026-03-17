using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Domain.Inputs;

public sealed record class GameRegisterDto
{
    [Required, MinLength(2)]
    public required string Name { get; init; } = default!;

    [Required, MinLength(2)]
    public required string Platform { get; init; } = default!;

    [Required, MinLength(2)]
    public required string PublisherName { get; init; } = default!;

    [Required, MinLength(2), MaxLength(100)]
    public required string Description { get; init; } = default!;

    [Required]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Preço deve ser maior que zero.")]
    public required decimal Price { get; init; } = default!;
}

