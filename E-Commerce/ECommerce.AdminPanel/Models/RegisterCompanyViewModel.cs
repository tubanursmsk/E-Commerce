using System.ComponentModel.DataAnnotations;

namespace ECommerce.AdminPanel.Models;

public class RegisterCompanyViewModel
{
    // Kişisel Bilgiler
    [Required(ErrorMessage = "Ad zorunludur.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçersiz email formatı.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    [Phone(ErrorMessage = "Geçersiz telefon numarası formatı.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    // Şirket Bilgiler
    [Required(ErrorMessage = "Şirket adı zorunludur.")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vergi numarası zorunludur.")]
    [StringLength(11, MinimumLength = 10, ErrorMessage = "Vergi No 10-11 haneli olmalıdır.")]
    public string TaxNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vergi dairesi zorunludur.")]
    public string TaxOffice { get; set; } = string.Empty;

    // Adres Bilgileri
    [Required(ErrorMessage = "Şehir seçimi zorunludur.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "İlçe zorunludur.")]
    public string District { get; set; } = string.Empty;

    public string? Street { get; set; }
    public string? ZipCode { get; set; }

    [Required(ErrorMessage = "Tam adres zorunludur.")]
    public string FullAddress { get; set; } = string.Empty;
}