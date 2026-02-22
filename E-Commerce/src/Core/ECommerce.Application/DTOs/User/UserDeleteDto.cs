using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.User;

    public class UserDeleteDto
    {
        [Required]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; } = true;
    }

