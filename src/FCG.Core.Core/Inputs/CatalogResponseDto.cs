using System.ComponentModel.DataAnnotations;

namespace FCG.Core.Core.Inputs
{
    public sealed record class CatalogResponseDto
    {
        [Required]
        public Guid UserId { get; init; } = default!;

        [Required]
        public Guid GameId { get; init; } = default!;

        [Required]
        public decimal Price { get; init; } = default!;
    }

}
