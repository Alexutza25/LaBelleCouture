namespace Try.DTO;

public class ProductFiltersDto
{
    public List<string>? Categories { get; set; }
    public List<string>? Colors { get; set; }
    public List<string>? Materials { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}