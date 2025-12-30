using ECommerce.Application.DTOs.Banner;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;

public interface IBannerService
{
    Task<ApiResponse<IEnumerable<BannerDto>>> GetAllAsync();
    Task<ApiResponse<Guid>> CreateAsync(BannerCreateDto dto);
    Task<ApiResponse<bool>> UpdateAsync(Guid id, BannerUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
   
}