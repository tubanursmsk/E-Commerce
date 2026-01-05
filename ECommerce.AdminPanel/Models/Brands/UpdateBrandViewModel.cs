using System.ComponentModel.DataAnnotations;

namespace ECommerce.AdminPanel.Models.Brands;

public class UpdateBrandViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Marka adı zorunludur.")]
    [Display(Name = "Marka Adı")]
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}
