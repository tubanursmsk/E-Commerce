using AutoMapper;
using ECommerce.Application.DTOs.Cargo;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class CargoService : ICargoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CargoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<CargoDto>>> GetAllAsync()
    {
        var cargoes = await _unitOfWork.Cargoes.GetAllAsync();
        return ApiResponse<IEnumerable<CargoDto>>.SuccessResult(_mapper.Map<IEnumerable<CargoDto>>(cargoes));
    }

    public async Task<ApiResponse<CargoDto>> GetByIdAsync(Guid id)
    {
        var cargo = await _unitOfWork.Cargoes.GetByIdAsync(id);
        if (cargo == null) return ApiResponse<CargoDto>.ErrorResult("Kargo firması bulunamadı.");
        return ApiResponse<CargoDto>.SuccessResult(_mapper.Map<CargoDto>(cargo));
    }

    public async Task<ApiResponse<Guid>> CreateAsync(CargoCreateDto dto)
    {
        var cargo = _mapper.Map<Cargo>(dto);
        await _unitOfWork.Cargoes.AddAsync(cargo);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<Guid>.SuccessResult(cargo.Id, "Kargo firması eklendi.");
    }

    public async Task<ApiResponse<bool>> UpdateAsync(Guid id, CargoUpdateDto dto)
    {
        var cargo = await _unitOfWork.Cargoes.GetByIdAsync(id);
        if (cargo == null) return ApiResponse<bool>.ErrorResult("Kargo firması bulunamadı.");

        _mapper.Map(dto, cargo);
        _unitOfWork.Cargoes.Update(cargo);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResult(true, "Kargo firması güncellendi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var cargo = await _unitOfWork.Cargoes.GetByIdAsync(id);
        if (cargo == null) return ApiResponse<bool>.ErrorResult("Kargo firması bulunamadı.");

        _unitOfWork.Cargoes.Delete(cargo);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResult(true, "Kargo firması silindi.");
    }
}