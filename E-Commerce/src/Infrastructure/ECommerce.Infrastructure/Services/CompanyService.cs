using AutoMapper;
using ECommerce.Application.DTOs.Company;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompanyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<CompanyDto>>> GetAllAsync()
    {
        var companies = await _unitOfWork.Companies.GetAllAsync();
        return ApiResponse<IEnumerable<CompanyDto>>.SuccessResult(_mapper.Map<IEnumerable<CompanyDto>>(companies));
    }

    public async Task<ApiResponse<CompanyDto>> GetByIdAsync(Guid id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null) return ApiResponse<CompanyDto>.ErrorResult("Şirket bulunamadı.");
        return ApiResponse<CompanyDto>.SuccessResult(_mapper.Map<CompanyDto>(company));
    }

    public async Task<ApiResponse<Guid>> CreateAsync(CompanyCreateDto dto)
    {
        var company = _mapper.Map<Company>(dto);
        // Yeni şirkete otomatik API Key üretiliyor (Entity içinde yapmıştık)
        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<Guid>.SuccessResult(company.Id, "Şirket başarıyla kaydedildi. Onay bekliyor.");
    }

    public async Task<ApiResponse<bool>> ApproveCompanyAsync(Guid id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null) return ApiResponse<bool>.ErrorResult("Şirket bulunamadı.");

        company.IsApproved = true;
        _unitOfWork.Companies.Update(company);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResult(true, "Şirket onaylandı.");
    }


    public async Task<ApiResponse<bool>> UpdateAsync(Guid id, CompanyUpdateDto dto)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null)
            return ApiResponse<bool>.ErrorResult("Güncellenecek şirket bulunamadı.");

        // AutoMapper ile DTO'daki yeni bilgileri mevcut entity üzerine yazıyoruz
        _mapper.Map(dto, company);
        company.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Companies.Update(company);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Şirket bilgileri güncellendi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null)
            return ApiResponse<bool>.ErrorResult("Silinecek şirket bulunamadı.");

        // Soft Delete (GenericRepository içinde IsDeleted = true yapmıştık)
        _unitOfWork.Companies.Delete(company);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Şirket başarıyla silindi.");
    }
}