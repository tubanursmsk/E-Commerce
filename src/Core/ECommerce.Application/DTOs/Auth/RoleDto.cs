using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Auth;

public class RoleDto // role assing dto --> rol atama 
{
    [Required(ErrorMessage = "UserId is required.")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "RoleName is required.")]
    public string RoleName { get; set; } = string.Empty;
}