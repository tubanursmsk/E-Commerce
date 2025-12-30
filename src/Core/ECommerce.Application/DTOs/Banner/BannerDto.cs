using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Banner;

    public class BannerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(500, ErrorMessage = "ImageUrl cannot exceed 500 characters.")]
        public string? ImageUrl { get; set; }

        [MaxLength(500, ErrorMessage = "TrackingUrlPrefix cannot exceed 500 characters.")]
        public string? TargetUrl { get; set; } // Banner'a tıklayınca nereye gidecek?
        public int Order { get; set; } // Banner sıralaması için yani Görüntülenme sırası

        public Guid CompanyId { get; set; }

        // BaseEntity
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }