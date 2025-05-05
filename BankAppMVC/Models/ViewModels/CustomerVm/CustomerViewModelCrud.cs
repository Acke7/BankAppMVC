using System.ComponentModel.DataAnnotations;

public class CustomerViewModelCrud
{
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(6)]
    public string Gender { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Givenname { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Surname { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Streetaddress { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = null!;

    [Required]
    [MaxLength(15)]
    public string Zipcode { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = null!;

    [Required]
    [MaxLength(2)]
    public string CountryCode { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateOnly? Birthday { get; set; }

    [MaxLength(20)]
    public string? NationalId { get; set; }

    [MaxLength(10)]
    public string? Telephonecountrycode { get; set; }

    [MaxLength(25)]
    public string? Telephonenumber { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? Emailaddress { get; set; }
}
