using System.ComponentModel.DataAnnotations;

namespace ECommerce.AdminPanel.Models.Categories;

public class UpdateCategoryViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [Display(Name = "Kategori Adı")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [Required]
    public Guid CompanyId { get; set; }
}