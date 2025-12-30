using ECommerce.Application.DTOs.Banner;
using ECommerce.Application.Interfaces;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiKey]
public class BannerController : ControllerBase
{
    private readonly IBannerService _bannerService;

    public BannerController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    [HttpGet("List")]
    [AllowAnonymous] // Banner'ları herkes görebilmeli
    public async Task<IActionResult> GetAll() => Ok(await _bannerService.GetAllAsync());

    [HttpPost("Create")]
    [Authorize(Roles = "Admin")] // Sadece Admin banner ekleyebilir
    public async Task<IActionResult> Create(BannerCreateDto dto) => Ok(await _bannerService.CreateAsync(dto));

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, BannerUpdateDto dto) => Ok(await _bannerService.UpdateAsync(id, dto));

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id) => Ok(await _bannerService.DeleteAsync(id));
}