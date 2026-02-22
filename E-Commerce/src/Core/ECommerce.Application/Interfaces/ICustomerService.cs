using ECommerce.Application.DTOs.Customer;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;

public interface ICustomerService
{
    Task<ApiResponse<CustomerDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<Guid>> CreateAsync(CustomerCreateDto dto);
    Task<ApiResponse<IEnumerable<CustomerDto>>> GetAllAsync(Guid? currentCompanyId, string role);
    Task<ApiResponse<IEnumerable<CustomerDto>>> SearchAsync(string keyword);
    Task<ApiResponse<bool>> UpdateProfileAsync(CustomerUpdateDto dto);
    Task<ApiResponse<CustomerDto>> GetProfileByUserIdAsync(Guid userId);
    
   
}