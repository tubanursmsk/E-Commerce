using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Request;

    public class RequestCreateDto
    {
        [Required, MinLength(2)]
        public string Subject { get; set; } = string.Empty;

        [Required, MinLength(5)]
        public string Message { get; set; } = string.Empty;

        [Required (ErrorMessage = "User ID is required.")]
        public Guid UserId { get; set; }

        [Required (ErrorMessage = "Company ID is required.")]
        public Guid CompanyId { get; set; }
    }
