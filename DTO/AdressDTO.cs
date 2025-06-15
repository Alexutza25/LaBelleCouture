using System.ComponentModel.DataAnnotations;

namespace Try.DTO;

public class AddressDto
{
    [Required(ErrorMessage = "County is required.")]
    public string County { get; set; }

    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; }

    [Required(ErrorMessage = "Street is required.")]
    public string Street { get; set; }

    [Required(ErrorMessage = "Number is required.")]
    public string Number { get; set; }

    [StringLength(10, ErrorMessage = "Building entrance must be max 10 characters.")]
    public string? BuildingEntrance { get; set; }

    [RegularExpression(@"^\d{0,2}$", ErrorMessage = "Floor must be a number up to 2 digits.")]
    public string? Floor { get; set; }

    [RegularExpression(@"^\d{0,4}$", ErrorMessage = "Apartment number must be a number up to 4 digits.")]
    public string? ApartmentNumber { get; set; }

    [StringLength(200, ErrorMessage = "Additional details must be under 200 characters.")]
    public string? AdditionalDetails { get; set; }
}