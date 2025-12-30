using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Review;
public class ReviewDeleteDto
{
    [Required]
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; } = true;

}