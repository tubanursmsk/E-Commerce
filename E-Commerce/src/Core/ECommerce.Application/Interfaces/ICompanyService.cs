using ECommerce.Application.DTOs.Company;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;

public interface ICompanyService
{
    Task<ApiResponse<IEnumerable<CompanyDto>>> GetAllAsync();
    Task<ApiResponse<CompanyDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<Guid>> CreateAsync(CompanyCreateDto dto);
    Task<ApiResponse<bool>> ApproveCompanyAsync(Guid id); // Şirketi onaylama (Yönerge gereği)
    Task<ApiResponse<bool>> UpdateAsync(Guid id, CompanyUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);

}