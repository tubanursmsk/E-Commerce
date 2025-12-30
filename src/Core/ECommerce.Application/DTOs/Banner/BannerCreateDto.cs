using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Banner
{
    public class BannerCreateDto
    {
        [Required, MinLength(2)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(500, ErrorMessage = "ImageUrl cannot exceed 500 characters.")]
        public string? ImageUrl { get; set; }

        [MaxLength(500, ErrorMessage = "TrackingUrlPrefix cannot exceed 500 characters.")]
        public string? TargetUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int Order { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
    }

}
