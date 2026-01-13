using System.ComponentModel.DataAnnotations;

namespace FIAP.FCG.CATALOG.Core.Inputs
{
    public sealed record class CatalogResponseDto
    {
        [Required]
        public int UserId { get; init; } = default!;

        [Required]
        public int GameId { get; init; } = default!;

        [Required]
        public decimal Price { get; init; } = default!;
    }

}
