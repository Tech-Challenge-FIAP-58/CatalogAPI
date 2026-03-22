using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Domain.Inputs
{
    public sealed record GameUpdateDto
    {
        [MinLength(2), MaxLength(100)]
        public string? Description { get; init; } = default!;

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Preço deve ser maior que zero.")]
        public decimal? Price { get; init; } = default!;

        public bool? IsAvailable { get; init; } = default!;
    }
}
