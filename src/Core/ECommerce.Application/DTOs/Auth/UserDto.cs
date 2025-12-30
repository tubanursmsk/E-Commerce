using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Auth;

public class UserDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;
    public List<string> Role { get; set; } = new List<string> { "Customer" }; // Kullanıcının sahip olduğu roller
}
