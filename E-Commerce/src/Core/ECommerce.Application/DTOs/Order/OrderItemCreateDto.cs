using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Order;

public class OrderItemCreateDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required (ErrorMessage = "Product name is required.")]
    public string ProductName { get; set; } = null!;

    [Required (ErrorMessage = "Quantity is required.")]
    [Range(1, 100, ErrorMessage = "Bir üründen en az 1, en fazla 100 adet sipariş verilebilir.")]
    public int Quantity { get; set; }

    [Required (ErrorMessage = "Price is required.")]
    public decimal Price { get; set; } // O anki satış fiyatı

    [Required (ErrorMessage = "Total amount is required.")]
    public decimal TotalAmount { get; set; }
}