using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Banner;

    public class BannerDeleteDto
    {
        [Required]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; } = true;
    }

