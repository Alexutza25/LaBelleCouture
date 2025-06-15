using System.ComponentModel.DataAnnotations;

namespace Try.DTO;

public class ProductVariantDto
{ 
    
    [Required]
    public int ProductId { get; set; }
    
    [Required(ErrorMessage = "Stock is required.")]
    public int Stock { get; set; }
    [Required(ErrorMessage = "Size is required.")]
    public string Size { get; set; }

}