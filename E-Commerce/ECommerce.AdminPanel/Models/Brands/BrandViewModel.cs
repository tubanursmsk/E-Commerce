using System.ComponentModel.DataAnnotations;

namespace ECommerce.AdminPanel.Models.Brands;
public class BrandViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Marka adı zorunludur.")]
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}

public class BrandListViewModel
{
    public IEnumerable<ECommerce.Application.DTOs.Brand.BrandDto> Brands { get; set; } = new List<ECommerce.Application.DTOs.Brand.BrandDto>();
}