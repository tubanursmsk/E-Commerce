using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.User;

public class UserUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyadı alanı zorunludur.")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";

    public Guid? CompanyId { get; set; }

    public bool Status { get; set; } = true;
}

