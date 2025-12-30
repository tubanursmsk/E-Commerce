using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Order;

public class OrderItemCreateDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string ProductName { get; set; } = null!;

    [Required]
    [Range(1, 100, ErrorMessage = "Bir üründen en az 1, en fazla 100 adet sipariş verilebilir.")]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; } // O anki satış fiyatı
}