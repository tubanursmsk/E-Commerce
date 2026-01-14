using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;

public interface IUserService
{
    // Kişisel profil güncelleme
    Task<ApiResponse<bool>> UpdateProfileAsync(Guid userId, UserUpdateDto dto);
    
    // Şirket yöneticisinin çalışanlarını görmesi
    Task<ApiResponse<IEnumerable<DTOs.User.UserDto>>> GetCompanyStaffAsync(Guid companyId);
    
    // Admin için global liste
    Task<ApiResponse<IEnumerable<DTOs.User.UserDto>>> GetAllUsersAsync();
    Task<ApiResponse<bool>> AssignRoleAsync(DTOs.User.UserDto dto);
    Task<ApiResponse<bool>> RemoveRoleAsync(DTOs.User.UserDto dto);
    Task<ApiResponse<DTOs.User.UserDto>> GetUserByIdAsync(Guid id);
}