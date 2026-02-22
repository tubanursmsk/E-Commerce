using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Cargo;

public class CargoCreateDto
{
    [Required, MinLength(2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tracking URL prefix is required.")]
    public string? TrackingUrlPrefix { get; set; }

    [Range(0, double.MaxValue)]
    public decimal BasePrice { get; set; }

    [Required (ErrorMessage = "Company ID is required.")]
    public Guid CompanyId { get; set; }
    public Guid OrderId { get; set; }
    public bool Status { get; set; }
}