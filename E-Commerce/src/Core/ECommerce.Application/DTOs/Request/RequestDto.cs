using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Request;

    public class RequestDto
    {
        public Guid Id { get; set; }

        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public string? Feedback { get; set; }
        public bool IsResolved { get; set; }

        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }

        // BaseEntity
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }


