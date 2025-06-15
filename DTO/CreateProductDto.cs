namespace Try.DTO;

public class CreateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Colour { get; set; }
    public string Material { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    public IFormFile? Image { get; set; }
}