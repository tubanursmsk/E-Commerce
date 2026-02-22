using ECommerce.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Order;

public class OrderSummaryDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Sipariş numarası zorunludur.")]
    public string OrderNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Toplam tutar zorunludur.")]
    public decimal TotalAmount { get; set; }
    public DateTime CreatedDate { get; set; }
    public OrderStatus Status { get; set; }
    public string CustomerFullName { get; set; } = string.Empty;
}