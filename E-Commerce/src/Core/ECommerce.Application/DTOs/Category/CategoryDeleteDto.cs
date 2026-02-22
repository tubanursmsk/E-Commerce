using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Category;

public class CategoryDeleteDto
{
        [Required]
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; } = true;
}