using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Brand;

public class BrandDto
{
    public Guid Id { get; set; } 

    [Required(ErrorMessage = "Marka adı zorunludur.")]
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; }= string.Empty;
    public Guid CompanyId { get; set; }
}