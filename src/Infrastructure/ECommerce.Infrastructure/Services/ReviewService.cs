using AutoMapper;
using ECommerce.Application.DTOs.Review;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<IEnumerable<ReviewDto>>> GetByProductIdAsync(Guid productId)
    {
        // Belirli bir ürüne ait silinmemiş yorumları getir
        var reviews = await _unitOfWork.Reviews.FindAsync(r => r.ProductId == productId);
        var dtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        return ApiResponse<IEnumerable<ReviewDto>>.SuccessResult(dtos);
    }

    public async Task<ApiResponse<ReviewDto>> GetByIdAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null) return ApiResponse<ReviewDto>.ErrorResult("Yorum bulunamadı.");

        var dto = _mapper.Map<ReviewDto>(review);
        return ApiResponse<ReviewDto>.SuccessResult(dto);
    }

    public async Task<ApiResponse<Guid>> CreateAsync(ReviewCreateDto dto)
    {
        // Puan kontrolü (Validation)
        if (dto.Rating < 1 || dto.Rating > 5)
            return ApiResponse<Guid>.ErrorResult("Puan 1 ile 5 arasında olmalıdır.");

        var review = _mapper.Map<Review>(dto);
        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<Guid>.SuccessResult(review.Id, "Yorumunuz başarıyla eklendi.");
    }

    public async Task<ApiResponse<bool>> UpdateAsync(Guid id, ReviewUpdateDto dto)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null) return ApiResponse<bool>.ErrorResult("Yorum bulunamadı.");

        // Puan kontrolü (Validation)
        if (dto.Rating < 1 || dto.Rating > 5)
            return ApiResponse<bool>.ErrorResult("Puan 1 ile 5 arasında olmalıdır.");

        // Güncelleme işlemi
        review.Comment = dto.Comment;
        review.Rating = dto.Rating;
        review.Status = dto.Status;
        review.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Reviews.Update(review);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Yorum başarıyla güncellendi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null) return ApiResponse<bool>.ErrorResult("Yorum bulunamadı.");

        _unitOfWork.Reviews.Delete(review);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResult(true, "Yorum silindi.");
    }
}