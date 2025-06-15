using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Try.Domain;

[Table("Cart")]
public class Cart
{
    [Key]
    public int CartId { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } 
    [ForeignKey("ProductVariant")]
    public int VariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }
    [Column("quantity")]
    public int Quantity { get; set; }
}