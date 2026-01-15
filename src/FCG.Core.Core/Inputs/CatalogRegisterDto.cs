using System.ComponentModel.DataAnnotations;

namespace FCG.Core.Core.Inputs
{
    public class CatalogRegisterDto
    {
        [Required]
        public int UserId { get; set; } = default!;

        [Required]
        public int GameId { get; set; } = default!;

        [Required]
        public decimal Price { get; set; } = default!;
    }
}
