namespace ECommerce.Application.DTOs.Product;

public class ProductFilterDto
{
    public Guid? CategoryId { get; set; }
    public List<Guid>? BrandIds { get; set; } // Çoklu marka seçimi için
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Keyword { get; set; } // Arama kelimesi
    public bool? IsFreeShipping { get; set; }
    public bool? IsFastDelivery { get; set; }
    
    // Sıralama: "price_asc", "price_desc", "newest", "name_asc"
    public string? SortBy { get; set; } 
    
    // Sayfalama (Pagination) - İleride lazım olur, şimdiden ekleyelim
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}