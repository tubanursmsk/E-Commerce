namespace ECommerce.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    // Yönergedeki şirket bazlı API Key gereksinimi için
    public string ApiKey { get; set; } = Guid.NewGuid().ToString("N");
    public bool IsApproved { get; set; } = false;


    // Navigation Properties (İlişkiler)
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}