namespace ECommerce.Domain.Entities;

public class Review : BaseEntity
{
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5 arası puan

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}