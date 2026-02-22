namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    // Rich Text editörden gelecek HTML metni için
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public bool IsFeatured { get; set; } = true;
    public decimal? DiscountPrice { get; set; } // İndirimli fiyat
    public bool IsFreeShipping { get; set; } // Ücretsiz kargo
    public bool IsFastDelivery { get; set; } // Hızlı teslimat

    // İlişkiler
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!; 
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public Guid BrandId { get; set; }
    public Brand Brand { get; set; } = null!;
}