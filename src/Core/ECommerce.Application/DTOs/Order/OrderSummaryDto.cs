using ECommerce.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Order;

public class OrderSummaryDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedDate { get; set; }
    public OrderStatus Status { get; set; }
    public string CustomerFullName { get; set; } = string.Empty;
}