using AutoMapper;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        return ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(dtos);
    }

    public async Task<ApiResponse<CategoryDto>> GetByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return ApiResponse<CategoryDto>.ErrorResult("Kategori bulunamadı.");
        
        var dto = _mapper.Map<CategoryDto>(category);
        return ApiResponse<CategoryDto>.SuccessResult(dto);
    }

    public async Task<ApiResponse<Guid>> CreateAsync(CategoryCreateDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<Guid>.SuccessResult(category.Id, "Kategori başarıyla oluşturuldu.");
    }

    public async Task<ApiResponse<bool>> UpdateAsync(Guid id, CategoryUpdateDto dto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return ApiResponse<bool>.ErrorResult("Kategori bulunamadı.");

        _mapper.Map(dto, category);
        category.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.SuccessResult(true, "Kategori güncellendi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return ApiResponse<bool>.ErrorResult("Kategori bulunamadı.");

        _unitOfWork.Categories.Delete(category); // Soft delete
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.SuccessResult(true, "Kategori silindi.");
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetByCompanyIdAsync(Guid companyId)
    {
        var categories = await _unitOfWork.Categories.FindAsync(x => x.CompanyId == companyId);
        var dtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        return ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(dtos);
    }
}

    
