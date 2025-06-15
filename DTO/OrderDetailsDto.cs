namespace Try.DTO;

public class OrderDetailsDto
{
    public int OrderDetailsId { get; set; }
    public int OrderId { get; set; }
    public int VariantId { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}