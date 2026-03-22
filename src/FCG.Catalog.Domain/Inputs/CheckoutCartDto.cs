using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Domain.Inputs;

public sealed record class CheckoutCartDto
{
    [Required]
    public int ClientId { get; init; }

    [Required]
    public PaymentMethod PaymentMethod { get; init; }

    [Required]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; init; }

    [Required]
    [MinLength(2)]
    public string CardName { get; init; } = default!;

    [Required]
    [MinLength(12)]
    public string CardNumber { get; init; } = default!;

    [Required]
    [MinLength(4)]
    public string ExpirationDate { get; init; } = default!;

    [Required]
    [MinLength(3)]
    public string Cvv { get; init; } = default!;
}

public enum PaymentMethod
{
    CreditCard = 1,
}
