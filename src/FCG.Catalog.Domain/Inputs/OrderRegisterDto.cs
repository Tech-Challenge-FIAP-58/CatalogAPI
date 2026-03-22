using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Domain.Inputs;

public sealed record class OrderRegisterDto
{
    [Required]
    public required DateTime OrderDate { get; set; }

    [Required]
    public required int UserId { get; set; }

    [Required]
    [MinLength(1)]
    public required List<OrderItemRegisterDto> OrderGames { get; set; }
}
