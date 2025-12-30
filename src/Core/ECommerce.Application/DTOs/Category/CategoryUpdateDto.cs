using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Category;
public class CategoryUpdateDto
{
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [MaxLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
    public string? Description { get; set; }

    [MaxLength(200, ErrorMessage = "Resim URL'si en fazla 200 karakter olabilir.")]
    public string? ImageUrl { get; set; }
}