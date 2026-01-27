using ECommerce.Application.DTOs.Product;

public class ProductListResponseDto
{
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    public int TotalCount { get; set; }
    
    // Sol panel için filtreleme seçenekleri (Facets)
    public IEnumerable<BrandFilterDto> AvailableBrands { get; set; } = new List<BrandFilterDto>();
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
}

public class BrandFilterDto { 
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}