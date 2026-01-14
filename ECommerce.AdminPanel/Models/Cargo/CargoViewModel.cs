using System.ComponentModel.DataAnnotations;
using ECommerce.Application.DTOs.Cargo;

namespace ECommerce.AdminPanel.Models.Cargo;

public class CargoViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Kargo firması adı zorunludur.")]
    [Display(Name = "Kargo Adı")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Takip URL Öneki")]
    public string? TrackingUrlPrefix { get; set; }

    [Required(ErrorMessage = "Sabit fiyat zorunludur.")]
    [Display(Name = "Standart Kargo Ücreti")]
    public decimal BasePrice { get; set; }

    public Guid CompanyId { get; set; }

}


public class CargoListViewModel
{
    public IEnumerable<CargoDto> Cargoes { get; set; } = new List<CargoDto>();
}


