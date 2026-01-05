using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;
public interface IBrandService 
{
     Task<ApiResponse<IEnumerable<BrandDto>>> GetAllAsync();
    Task<ApiResponse<BrandDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<Guid>> CreateAsync(BrandCreateDto dto);
    Task<ApiResponse<bool>> UpdateAsync(Guid id, BrandUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
    Task<ApiResponse<IEnumerable<BrandDto>>> GetByCompanyIdAsync(Guid companyId);
}

