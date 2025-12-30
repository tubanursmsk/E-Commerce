using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Product;

public class ProductCreateDto
{
    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ürün açıklaması zorunludur.")]
    public string Description { get; set; } = string.Empty; // Yönergedeki Rich Text HTML gelecek

    [Required(ErrorMessage = "Fiyat zorunludur.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
    public decimal Price { get; set; } //Fiyat gereksinimleri dahil edildi

    [Required(ErrorMessage = "Stok adedi zorunludur.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok negatif olamaz.")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Şirket ID zorunludur.")]
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "Marka ID zorunludur.")]
    public Guid BrandId { get; set; }
    public string? ImageUrl { get; set; }
}