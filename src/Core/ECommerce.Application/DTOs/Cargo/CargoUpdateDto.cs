using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Cargo;

public class CargoUpdateDto
{
    [Required, MinLength(2)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "TrackingUrlPrefix cannot exceed 500 characters.")]
    public string? TrackingUrlPrefix { get; set; }

    [Range(0, double.MaxValue)]
    public decimal BasePrice { get; set; }

    [Required]
    public Guid CompanyId { get; set; }
    public Guid OrderId { get; set; }
    public bool Status { get; set; } = true;
}