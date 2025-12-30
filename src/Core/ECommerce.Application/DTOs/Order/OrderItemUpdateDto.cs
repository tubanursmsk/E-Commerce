using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Order;

public class OrderItemUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string ProductName { get; set; } = null!;

    [Required(ErrorMessage = "Miktar zorunludur.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Fiyat zorunludur.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
    public decimal Price { get; set; }
}