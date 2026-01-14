using System.ComponentModel.DataAnnotations;

namespace FIAP.FCG.CATALOG.Core.Inputs;

public sealed record class OrderRegisteredDto
{

    public int ClientId { get; set; }
    public int OrderId { get; set; }
    public int PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string CardName { get; set; }
    public string CardNumber { get; set; }
    public string ExpirationDate { get; set; }
    public string Cvv { get; set; }

    /*
    public  int OrderId { get; set; }
    public  DateTime OrderDate { get; set; }
    public  int UserId { get; set; }
    public  int GameId { get; set; }
    public  Decimal Price { get; set; }
    public  string PaymentStatus { get; set; }
    public  string CardName { get; set; }
    public  string CardNumber { get; set; }
    public  string ExpirationDate { get; set; }
    public  string Cvv { get; set; }*/

}
 
