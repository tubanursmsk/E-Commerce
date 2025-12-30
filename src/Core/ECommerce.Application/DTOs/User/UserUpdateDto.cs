using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.User;

    public class UserUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required, MinLength(2)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MinLength(2)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Customer";

        public Guid? CompanyId { get; set; }

        public bool Status { get; set; } = true;
    }

   