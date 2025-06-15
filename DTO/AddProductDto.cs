public class AddProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string Colour { get; set; }
    public string Material { get; set; }
    public IFormFile? ImageURL { get; set; } 
}