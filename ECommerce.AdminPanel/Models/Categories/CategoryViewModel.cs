using System.ComponentModel.DataAnnotations;

namespace ECommerce.AdminPanel.Models.Categories;

public class CategoryViewModel
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    
    public Guid CompanyId { get; set; }
}

public class CategoryListViewModel
{
    public IEnumerable<ECommerce.Application.DTOs.Category.CategoryDto> Categories { get; set; } = new List<ECommerce.Application.DTOs.Category.CategoryDto>();
}