using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Review;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Interfaces;
public interface IReviewService 
{
    Task<ApiResponse<IEnumerable<ReviewDto>>> GetByProductIdAsync(Guid productId);
    Task<ApiResponse<Guid>> CreateAsync(ReviewCreateDto dto);
    Task<ApiResponse<bool>> UpdateAsync(Guid id, ReviewUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);

}

