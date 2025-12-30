namespace ECommerce.Domain.Entities;

public class Customer : BaseEntity
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string City { get; set; } = string.Empty;

    // Her müşteri bir sisteme giriş kullanıcısına (User) bağlıdır
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Müşterinin sipariş geçmişi
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}