using AutoMapper;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /*public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync()
  {

      //var products = await _unitOfWork.Products.GetAllAsync();
      //var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
      //return ApiResponse<IEnumerable<ProductDto>>.SuccessResult(dtos);
      var all = await _unitOfWork.Products.GetAllWithCategoryAsync();
      //var brandAll = await _unitOfWork.Products.GetAllWithBrandAsync();
      return ApiResponse<IEnumerable<ProductDto>>.SuccessResult(_mapper.Map<IEnumerable<ProductDto>>(all));
}
      */

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync()
    {
        // Veritabanından hem Category hem de Brand bilgilerini tek seferde çekiyoruz
        var products = await _unitOfWork.Products.GetAllWithCategoryAndBrandAsync();

        // Mapper zaten Product içindeki Category ve Brand nesnelerini ProductDto'ya eşleyecektir
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        return ApiResponse<IEnumerable<ProductDto>>.SuccessResult(dtos);
    }


    public async Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return ApiResponse<ProductDto>.ErrorResult("Ürün bulunamadı.");

        var dto = _mapper.Map<ProductDto>(product);
        return ApiResponse<ProductDto>.SuccessResult(dto);
    }
    public async Task<ApiResponse<IEnumerable<ProductDto>>> SearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return await GetAllAsync();

        var products = await _unitOfWork.Products.FindAsync(p =>
            p.Name.ToLower().Contains(keyword.ToLower()) ||
            (p.Description != null && p.Description.ToLower().Contains(keyword.ToLower())));

        if (products == null || !products.Any())
        {
            // Başarılı ama sonuç yok mesajı (İstersen ErrorResult da dönebilirsin)
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResult(new List<ProductDto>(), $"'{keyword}' aramasıyla eşleşen ürün bulunamadı.");
        }

        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return ApiResponse<IEnumerable<ProductDto>>.SuccessResult(dtos);
    }
    public async Task<ApiResponse<Guid>> CreateAsync(ProductCreateDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<Guid>.SuccessResult(product.Id, "Ürün başarıyla eklendi.");
    }

    public async Task<ApiResponse<bool>> UpdateAsync(Guid id, ProductUpdateDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return ApiResponse<bool>.ErrorResult("Güncellenecek ürün bulunamadı.");

        _mapper.Map(dto, product); // DTO'daki verileri mevcut entity üzerine yazar
        product.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Ürün güncellendi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return ApiResponse<bool>.ErrorResult("Silinecek ürün bulunamadı.");

        _unitOfWork.Products.Delete(product); // GenericRepository'de IsDeleted = true yapacak
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Ürün silindi.");
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetByCompanyIdAsync(Guid companyId)
    {
        // IProductRepository içinde özel metod yazılabilirdi ama şimdilik FindAsync ile çözelim
        var products = await _unitOfWork.Products.FindAsync(x => x.CompanyId == companyId);
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return ApiResponse<IEnumerable<ProductDto>>.SuccessResult(dtos);
    }
}