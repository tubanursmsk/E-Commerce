using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<string>> LoginAsync(LoginDto dto);
    Task<ApiResponse<Guid>> RegisterWithCompanyAsync(RegisterCompanyDto dto);
}