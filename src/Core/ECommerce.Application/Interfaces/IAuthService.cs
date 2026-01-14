using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;
public interface IAuthService {
    Task<ApiResponse<string>> LoginAsync(LoginDto dto);
    Task<ApiResponse<Guid>> RegisterWithCompanyAsync(RegisterCompanyDto dto);
    Task<ApiResponse<Guid>> RegisterForCompanyAsync(RegisterDto dto, Guid companyId);
    Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto);
   
}