namespace Try.DTO;

public class ProductDetailsDto
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Colour { get; set; }
    public string Material { get; set; }
    public decimal Price { get; set; }
    public string ImageURL { get; set; }
    public string Category { get; set; } 
    
    public int CategoryId { get; set; }

}