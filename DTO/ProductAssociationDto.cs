namespace Try.DTO;

public class ProductAssociationDto
{
    public int Product1Id { get; set; }
    public int Product2Id { get; set; }
    public int Count { get; set; }
    
    
    public ProductDetailsDto? Product1 { get; set; }
    public ProductDetailsDto Product2 { get; set; }
}
