using System.ComponentModel.DataAnnotations;
using ECommerce.Application.DTOs.Cargo;

namespace ECommerce.AdminPanel.Models.Cargo;

public class UpdateCargoViewModel
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Kargo adı zorunludur.")]
    public string Name { get; set; } = string.Empty;
    public string? TrackingUrlPrefix { get; set; }
    [Required(ErrorMessage = "Fiyat zorunludur.")]
    public decimal BasePrice { get; set; }
    public bool Status { get; set; }
    public Guid CompanyId { get; set; }
}