using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Banner;

    public class BannerUpdateDto
    {
        [Required, MinLength(2)]
        public string Title { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [MaxLength(500, ErrorMessage = "TrackingUrlPrefix cannot exceed 500 characters.")]
        public string? TargetUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int Order { get; set; } //reklam görüntüle sırası

        [Required]
        public Guid CompanyId { get; set; }
        public bool Status { get; set; } = true;
    }


