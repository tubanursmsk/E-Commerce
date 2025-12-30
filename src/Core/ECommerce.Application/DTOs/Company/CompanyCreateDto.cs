using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Company;

public class CompanyCreateDto
{
    [Required(ErrorMessage = "Şirket adı zorunludur.")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vergi numarası zorunludur.")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Vergi numarası 10 haneli olmalıdır.")]
    public string TaxNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    [Phone(ErrorMessage = "Geçersiz telefon formatı.")]
    public string Phone { get; set; } = string.Empty;

    public string FullAddress { get; set; } = string.Empty;
}

// ApiKey'yi genelde backend üretir; dışarıdan set ettirmemek daha güvenli.
 // IsApproved da genelde admin onayıyla değişir; create'te false kalmalı.