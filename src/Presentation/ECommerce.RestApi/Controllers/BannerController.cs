using System.Security.Claims;
using ECommerce.Application.DTOs.Banner;
using ECommerce.Application.Interfaces;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
//[ApiKey]
public class BannerController : ControllerBase
{
    private readonly IBannerService _bannerService;

    public BannerController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    [HttpGet("List")]
    [Authorize] // Listeleme için yetki istiyoruz ki rolü kontrol edebilelim
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var companyIdStr = User.FindFirstValue("companyId");
        Guid? companyId = string.IsNullOrEmpty(companyIdStr) ? null : Guid.Parse(companyIdStr);

        return Ok(await _bannerService.GetAllAsync(companyId, role));
    }
    
    [HttpPost("Create")]
    [Authorize(Roles = "Admin,CompanyManager")] // Sadece Admin banner ekleyebilir
    public async Task<IActionResult> Create(BannerCreateDto dto) => Ok(await _bannerService.CreateAsync(dto));

     
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _bannerService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Update(Guid id, BannerUpdateDto dto) => Ok(await _bannerService.UpdateAsync(id, dto));

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Delete(Guid id) => Ok(await _bannerService.DeleteAsync(id));
}