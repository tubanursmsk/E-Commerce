using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Category;

public class CategoryDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }

        // BaseEntity
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
}