using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Brand;

public class BrandUpdateDto
{
    [Required(ErrorMessage = "Marka adı zorunludur.")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }
   
}