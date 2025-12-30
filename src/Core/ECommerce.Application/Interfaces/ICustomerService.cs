using ECommerce.Application.DTOs.Customer;
using ECommerce.Application.Responses;

public interface ICustomerService
{
    Task<ApiResponse<Guid>> CreateAsync(CustomerCreateDto dto);
    Task<ApiResponse<IEnumerable<CustomerDto>>> GetAllAsync();
    Task<ApiResponse<IEnumerable<CustomerDto>>> SearchAsync(string keyword);
}

