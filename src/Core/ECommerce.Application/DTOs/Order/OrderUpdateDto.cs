using ECommerce.Domain.Enums;
using System.ComponentModel.DataAnnotations;
namespace ECommerce.Application.DTOs.Order;

public class OrderUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    // Genelde siparişte update işlemi olarak status update kullanılır
    [Required]
    public OrderStatus Status { get; set; }
}