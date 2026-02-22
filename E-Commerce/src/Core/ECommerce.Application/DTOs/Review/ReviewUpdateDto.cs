using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Review;

public class ReviewUpdateDto
{

    [Required(ErrorMessage = "Yorum metni boş bırakılamaz.")]
    [MinLength(10, ErrorMessage = "Yorum en az 10 karakter olmalıdır.")]
    public string Comment { get; set; } = string.Empty;

    [Required]
    [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
    public int Rating { get; set; }

    public bool Status { get; set; } = true;
}