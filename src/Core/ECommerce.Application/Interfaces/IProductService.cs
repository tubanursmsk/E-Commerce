using System.Xml.Serialization;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync();
    Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<IEnumerable<ProductDto>>> SearchAsync(string keyword);
    Task<ApiResponse<Guid>> CreateAsync(ProductCreateDto dto);
    Task<ApiResponse<bool>> UpdateAsync(Guid id, ProductUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
    
    // Yönerge Gereği: Arama ve Şirket bazlı filtreleme
    Task<ApiResponse<IEnumerable<ProductDto>>> GetByCompanyIdAsync(Guid companyId);
}