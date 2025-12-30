namespace ECommerce.Application.DTOs.Order;
using System.ComponentModel.DataAnnotations;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }

     [Required(ErrorMessage = "Ürün adını yazınız.")]
    public string ProductName { get; set; } = null!;

    [Required(ErrorMessage = "Miktar zorunludur.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Fiyat zorunludur.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
    public decimal Price { get; set; }
    public decimal TotalPrice => Quantity * Price;

     // BaseEntity
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
}