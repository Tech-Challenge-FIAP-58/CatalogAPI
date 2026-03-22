using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Domain.Inputs
{
    public sealed record OrderUpdateDto
    {
        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        [MinLength(1)]
        public required List<OrderItemRegisterDto> OrderGames { get; set; }
    }
}
