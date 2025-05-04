using System.ComponentModel.DataAnnotations;

public class CustomerViewModelCrud
{
    public int CustomerId { get; set; }

    
    [Display(Name = "Gender")]
    public string Gender { get; set; } = null!;

    public string? Givenname { get; set; }

    public string? Surname { get; set; }

    public string? Streetaddress { get; set; }

    public string? City { get; set; }

    public string? Zipcode { get; set; }

    [Required]
    [Display(Name = "Country")]
    public string Country { get; set; } = null!;

    public string? CountryCode { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? Birthday { get; set; }

    public string? NationalId { get; set; }

    public string? Telephonecountrycode { get; set; }

    [Phone]
    public string? Telephonenumber { get; set; }

    [EmailAddress]
    public string? Emailaddress { get; set; }
}
