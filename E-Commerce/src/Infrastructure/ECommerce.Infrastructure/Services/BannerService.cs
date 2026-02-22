using AutoMapper;
using ECommerce.Application.DTOs.Banner;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq; // OrderBy ve filtreleme için

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

    public async Task<ApiResponse<IEnumerable<BannerDto>>> GetAllAsync(Guid? companyId, string? role)
    {
        // 1. Önce tüm bannerları çekelim (veya IsDeleted kontrolü ile)
        var allBanners = await _unitOfWork.Banners.GetAllAsync();

        // 2. IQueryable yerine veriyi bellek üzerinde (In-Memory) filtreleyelim (Hataları önlemek için)
        var query = allBanners.Where(b => !b.IsDeleted && b.Status);

        var result = query.OrderBy(b => b.Order).ToList();
        var dtos = _mapper.Map<IEnumerable<BannerDto>>(result);
        return ApiResponse<IEnumerable<BannerDto>>.SuccessResult(dtos);
    }
    public async Task<ApiResponse<BannerDto>> GetByIdAsync(Guid id)
    {
        var banner = await _unitOfWork.Banners.GetByIdAsync(id);
        if (banner == null) return ApiResponse<BannerDto>.ErrorResult("Banner bulunamadı.");
        return ApiResponse<BannerDto>.SuccessResult(_mapper.Map<BannerDto>(banner));
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