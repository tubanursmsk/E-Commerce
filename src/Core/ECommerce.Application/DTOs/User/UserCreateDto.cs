using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.User;

    // Admin ekranından kullanıcı oluşturma gibi düşün
    public class UserCreateDto
    {
        [Required, MinLength(2)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MinLength(2)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty; // hashlenip PasswordHash'e yazılacak

        [Required]
        public string Role { get; set; } = "Customer";

        public Guid? CompanyId { get; set; }
    }