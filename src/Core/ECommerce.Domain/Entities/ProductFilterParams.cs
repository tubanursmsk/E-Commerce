namespace ECommerce.Domain.Models;

public class ProductFilterParams
{
    public Guid? CategoryId { get; set; }
    public List<Guid>? BrandIds { get; set; } // Çoklu marka seçimi için
    public decimal? MinPrice { get; set; } // Product.Price > Filter.MinPrice mantığınıkurgulamak için 
    public decimal? MaxPrice { get; set; }
    public string? Keyword { get; set; }
    public bool? IsFreeShipping { get; set; }
    public bool? IsFastDelivery { get; set; }
    
    // Sıralama
    public string? SortBy { get; set; } 
    
    // Sayfalama
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}