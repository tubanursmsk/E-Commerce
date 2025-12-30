using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Request;

    // Kullanıcı kendi talebini güncelleyecekse
    public class RequestUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required, MinLength(2)]
        public string Subject { get; set; } = string.Empty;

        [Required, MinLength(5)]
        public string Message { get; set; } = string.Empty;

        public bool Status { get; set; } = true;
    }

