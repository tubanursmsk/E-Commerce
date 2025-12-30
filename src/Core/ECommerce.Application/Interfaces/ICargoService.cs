using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Cargo;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;
public interface ICargoService 
{
    Task<ApiResponse<IEnumerable<CargoDto>>> GetAllAsync();
    Task<ApiResponse<CargoDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<Guid>> CreateAsync(CargoCreateDto dto);
    Task<ApiResponse<bool>> UpdateAsync(Guid id, CargoUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}

