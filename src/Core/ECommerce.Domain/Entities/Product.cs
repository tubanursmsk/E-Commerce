namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    // Rich Text editörden gelecek HTML metni için
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }

    // İlişkiler
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!; 

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public Guid BrandId { get; set; }
    public Brand Brand { get; set; } = null!;
}