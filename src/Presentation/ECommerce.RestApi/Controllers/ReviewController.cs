using ECommerce.Application.DTOs.Review;
using ECommerce.Application.Interfaces;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("Product/{productId}")]
    [AllowAnonymous] // Yorumları herkes okuyabilir
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        return Ok(await _reviewService.GetByProductIdAsync(productId));
    }

    [HttpPost("Create")]
    [Authorize] // Sadece giriş yapmış kullanıcılar yorum yapabilir
    public async Task<IActionResult> Create(ReviewCreateDto dto)
    {
        return Ok(await _reviewService.CreateAsync(dto));
    }
     
     [HttpPost("Update/{id}")]
    [Authorize] // Sadece giriş yapmış kullanıcılar yorum yapabilir
    public async Task<IActionResult> Update(Guid id, ReviewUpdateDto dto)
    {
        return Ok(await _reviewService.UpdateAsync(id, dto));
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin")] // Yorumları sadece Admin silebilir (Moderatör mantığı)
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok(await _reviewService.DeleteAsync(id));
    }
}