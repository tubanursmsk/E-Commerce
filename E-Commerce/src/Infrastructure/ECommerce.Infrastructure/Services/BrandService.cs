using AutoMapper;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class BrandService : IBrandService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BrandService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<BrandDto>>> GetAllAsync()
    {
        var brands = await _unitOfWork.Brands.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<BrandDto>>(brands);
        return ApiResponse<IEnumerable<BrandDto>>.SuccessResult(dtos);
    }

    public async Task<ApiResponse<BrandDto>> GetByIdAsync(Guid id)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(id);
        if (brand == null) return ApiResponse<BrandDto>.ErrorResult("Marka bulunamadı.");
        
        return ApiResponse<BrandDto>.SuccessResult(_mapper.Map<BrandDto>(brand));
    }

    public async Task<ApiResponse<Guid>> CreateAsync(BrandCreateDto dto)
    {
        var brand = _mapper.Map<Brand>(dto);
        await _unitOfWork.Brands.AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<Guid>.SuccessResult(brand.Id, "Marka başarıyla eklendi.");
    }

    public async Task<ApiResponse<bool>> UpdateAsync(Guid id, BrandUpdateDto dto)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(id);
        if (brand == null) return ApiResponse<bool>.ErrorResult("Marka bulunamadı.");

        _mapper.Map(dto, brand);
        brand.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Brands.Update(brand);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.SuccessResult(true, "Marka güncellendi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(id);
        if (brand == null) return ApiResponse<bool>.ErrorResult("Marka bulunamadı.");

        _unitOfWork.Brands.Delete(brand); // Soft delete tetiklenir
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.SuccessResult(true, "Marka silindi.");
    }

    public async Task<ApiResponse<IEnumerable<BrandDto>>> GetByCompanyIdAsync(Guid companyId)
    {
        var brands = await _unitOfWork.Brands.FindAsync(x => x.CompanyId == companyId);
        var dtos = _mapper.Map<IEnumerable<BrandDto>>(brands);
        return ApiResponse<IEnumerable<BrandDto>>.SuccessResult(dtos);
    }
}