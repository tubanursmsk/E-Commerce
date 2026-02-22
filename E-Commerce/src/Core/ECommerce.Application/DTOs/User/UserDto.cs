using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.User;

public class UserDto
{
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

    // Basit rolde string var
    public string Role { get; set; } = "Staff";

    public Guid? CompanyId { get; set; }

    // BaseEntity
    public bool Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}