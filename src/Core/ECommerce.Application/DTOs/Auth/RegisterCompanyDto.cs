using System.ComponentModel.DataAnnotations;

public class RegisterCompanyDto
{
    // Kullanıcı Bilgileri
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", 
     ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
    public string Password { get; set; } = string.Empty;

    // Şirket Bilgileri

    [Required(ErrorMessage = "Company name is required.")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tax number is required.")]
    public string TaxNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "District is required.")]
    public string District { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full address is required.")]
    public string FullAddress { get; set; } = string.Empty;
}