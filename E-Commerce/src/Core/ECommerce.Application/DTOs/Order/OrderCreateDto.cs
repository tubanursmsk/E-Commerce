using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Order;

public class OrderCreateDto
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Siparişte en az bir ürün bulunmalıdır.")]
    public List<OrderItemCreateDto> OrderItems { get; set; } = new();
}