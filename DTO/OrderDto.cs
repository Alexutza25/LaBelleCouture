using System.ComponentModel.DataAnnotations;

namespace Try.DTO;

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
    
    [RegularExpression("^(CASH|CARD)$", ErrorMessage = "Payment Method must be 'CASH' or 'CARD'.")]
    public string PaymentMethod { get; set; }
}