namespace Try.Domain;

public class Address
{
    public int AddressId { get; set; }
    public string County { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string Number { get; set; }
    public string? BuildingEntrance { get; set; }  
    public string? Floor { get; set; }
    public string? ApartmentNumber { get; set; }   
    public string? AdditionalDetails { get; set; } 
}
