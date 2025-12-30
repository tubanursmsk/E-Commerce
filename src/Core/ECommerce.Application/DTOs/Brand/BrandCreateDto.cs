using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Brand;

public class BrandCreateDto
{
    [Required(ErrorMessage = "Marka adı zorunludur.")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    [Required(ErrorMessage = "Şirket ID zorunludur.")]
    public Guid CompanyId { get; set; }
}
