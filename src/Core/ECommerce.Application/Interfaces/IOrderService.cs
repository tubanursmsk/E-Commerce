using ECommerce.Application.DTOs.Order;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<IEnumerable<OrderDto>>> GetAllAsync();
    Task<ApiResponse<OrderDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<IEnumerable<OrderDto>>> SearchByOrderNumberAsync(string orderNumber);
    Task<ApiResponse<Guid>> CreateOrderAsync(OrderCreateDto dto);
    Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, ECommerce.Domain.Enums.OrderStatus status);
}