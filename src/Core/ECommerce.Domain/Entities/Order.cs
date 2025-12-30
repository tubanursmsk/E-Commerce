using ECommerce.Domain.Enums; 
namespace ECommerce.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    
    // Sipariş durumu (Beklemede, Hazırlanıyor, Kargolandı vb.)
    public new OrderStatus Status { get; set; } = OrderStatus.Pending; 

    // İlişkiler
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}