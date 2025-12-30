using AutoMapper;
using ECommerce.Application.DTOs.Banner;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class BannerService : IBannerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BannerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<BannerDto>>> GetAllAsync()
    {
        var banners = await _unitOfWork.Banners.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<BannerDto>>(banners.OrderBy(x => x.Order));
        return ApiResponse<IEnumerable<BannerDto>>.SuccessResult(dtos);
    }

    public async Task<ApiResponse<Guid>> CreateAsync(BannerCreateDto dto)
    {
        var banner = _mapper.Map<Banner>(dto);
        await _unitOfWork.Banners.AddAsync(banner);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<Guid>.SuccessResult(banner.Id, "Banner başarıyla eklendi.");
    }

    public async Task<ApiResponse<bool>> UpdateAsync(Guid id, BannerUpdateDto dto)
    {
        var banner = await _unitOfWork.Banners.GetByIdAsync(id);
        if (banner == null) return ApiResponse<bool>.ErrorResult("Banner bulunamadı.");

        _mapper.Map(dto, banner);
        _unitOfWork.Banners.Update(banner);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResult(true, "Banner güncellendi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var banner = await _unitOfWork.Banners.GetByIdAsync(id);
        if (banner == null) return ApiResponse<bool>.ErrorResult("Banner bulunamadı.");

        _unitOfWork.Banners.Delete(banner);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResult(true, "Banner silindi.");
    }
}