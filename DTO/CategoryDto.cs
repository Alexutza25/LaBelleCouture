using System.ComponentModel.DataAnnotations;

namespace Try.DTO;

public class CategoryDto
{
    [Required(ErrorMessage = "Category name is required.")]
    public string Name { get; set; }
    
}