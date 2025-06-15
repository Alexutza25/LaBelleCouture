using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Try.Domain;

[Table("Order")] 
public class Order
{
    [Key]
    public int OrderId { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } 
    [Column("date")]
    public DateTime Date { get; set; } = DateTime.Now;
    [Column("total")]
    public decimal Total { get; set; }
    [Column("status")]
    public string Status { get; set; } = "Pending";
    [Column("payment_method")]
    public string PaymentMethod { get; set; }
    public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

}