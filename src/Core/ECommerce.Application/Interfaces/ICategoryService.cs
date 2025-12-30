using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;

public interface ICategoryService
{
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<ApiResponse<CategoryDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<Guid>> CreateAsync(CategoryCreateDto dto);
    Task<ApiResponse<bool>> UpdateAsync(Guid id, CategoryUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetByCompanyIdAsync(Guid companyId);
}
