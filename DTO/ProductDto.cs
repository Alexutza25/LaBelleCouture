using System.ComponentModel.DataAnnotations;

namespace Try.DTO;

public class ProductDto
{
    [Required(ErrorMessage = "Product name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Product description is required.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Category ID is required.")]
    public int CategoryId { get; set; }

    
    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Colour is required.")]
    public string Colour { get; set; }
    [Required(ErrorMessage = "Material is required.")]
    public string Material { get; set; }

    
    public string? ImageURL { get; set; }
    
    public int? ProductId { get; set; } 

}