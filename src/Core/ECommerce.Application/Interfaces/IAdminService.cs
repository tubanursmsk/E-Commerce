using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;

public interface IAdminService
{
    Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync();
    Task<ApiResponse<bool>> AssignRoleAsync(RoleDto dto);
    Task<ApiResponse<bool>> RemoveRoleAsync(RoleDto dto);
}