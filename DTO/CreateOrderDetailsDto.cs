using System.ComponentModel.DataAnnotations;

namespace Try.DTO;

public class CreateOrderDetailsDto
{
    [Required(ErrorMessage = "Variant id is required.")]
    public int VariantId { get; set; }
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 1.")]
    
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}
