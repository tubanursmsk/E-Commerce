namespace ECommerce.Domain.Entities;

public class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }

    // Her marka bir şirkete aittir
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>(); // Bir markanın birden çok ürünü olabilir
}