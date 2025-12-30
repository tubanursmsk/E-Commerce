using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.User;

    public class UserDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // Basit rolde string var
        public string Role { get; set; } = "Customer";

        public Guid? CompanyId { get; set; }

        // BaseEntity
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }