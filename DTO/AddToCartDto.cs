using System.ComponentModel.DataAnnotations;

namespace Try.DTO;

public class AddToCartDto
{
    [Required(ErrorMessage = "Variants Id is required.")]
    public int ProductVariantId { get; set; }
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quntity must be greater than 1.")]
    public int Quantity { get; set; }
}