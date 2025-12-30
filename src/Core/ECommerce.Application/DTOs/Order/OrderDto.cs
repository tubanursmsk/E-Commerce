using ECommerce.Domain.Enums;
using System.ComponentModel.DataAnnotations;
namespace ECommerce.Application.DTOs.Order;

public class OrderDto
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Sipariş numarası zorunludur.")]
    public string OrderNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Toplam tutar zorunludur.")]
    public decimal TotalAmount { get; set; }
    [Required(ErrorMessage = "Sipariş durumu zorunludur.")]
    public OrderStatus Status { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Siparişte en az bir ürün bulunmalıdır.")]

    public List<OrderItemDto> OrderItems { get; set; } = new();


    // BaseEntity
    public bool StatusFlag { get; set; } // çakışma olmasın diye isim verdim (aşağıdaki notu oku)
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

// NOT: BaseEntity'de bool Status var, Order içinde de OrderStatus Status var.
    // İkisi isim çakıştığı için OrderDto içinde BaseEntity'deki Status'u "StatusFlag" yaptım.
    // İstersen BaseEntity'deki Status adını "IsActive" yaparak bu çakışmayı tamamen bitirebilirsin.

    /*
    Senin BaseEntity’de bool Status var.
Order entity’sinde de OrderStatus Status var.

Bu, DTO’da veya AutoMapper’da kafa karıştırır. Ben OrderDto içinde BaseEntity’deki Status alanını StatusFlag yaptım.

✅ En temiz çözüm: BaseEntity’deki Status adını IsActive gibi bir şeye çevirmen.
(Temiz mimaride bu tip isim çakışmaları ileride çok can sıkıyor.)
    */