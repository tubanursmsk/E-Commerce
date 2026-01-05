using System.ComponentModel.DataAnnotations;

namespace ECommerce.AdminPanel.Models.Products;
public class UpdateProductViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lütfen bir kategori seçin.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Lütfen bir marka seçin.")]
    public Guid BrandId { get; set; }

    [Required]
    public Guid CompanyId { get; set; } 
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Fiyat zorunludur.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stok zorunludur.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok negatif olamaz.")]
    public int Stock { get; set; }
}