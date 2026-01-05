using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Product;

public class ProductDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;    
    // Rich Text HTML
    [Required(ErrorMessage = "Ürün açıklaması zorunludur.")]
    [MaxLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Fiyat zorunludur.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stok adedi zorunludur.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok negatif olamaz.")]
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }

    public Guid CategoryId { get; set; }
    public Guid BrandId { get; set; }
    public Guid CompanyId { get; set; }
    
    // İlişkili tablodan sadece isim çekiyoruz (Performans dostu)

     [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    public string CategoryName { get; set; } = string.Empty;

     [Required(ErrorMessage = "Marka seçimi zorunludur.")]
    public string BrandName { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;
}