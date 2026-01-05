using System.Security.Claims;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
//[ApiKey] // X-Api-Key Header'ı zorunlu
[Authorize] // Geçerli JWT Token zorunlu
public class BrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    /*[HttpGet("List")]
    [AllowAnonymous] // Markaları herkes görebilsin (Katalog amaçlı)
    public async Task<IActionResult> GetAll()
    {
        var result = await _brandService.GetAllAsync();
        return Ok(result);
    }*/

    [HttpGet("List")]
    public async Task<IActionResult> GetAll()

    {
        // 1. Kullanıcının rolünü alalım
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        // 2. Eğer kullanıcı Admin ise tüm ürünleri getir
        if (userRole == "Admin")
        {
            var result = await _brandService.GetAllAsync();
            return Ok(result);
        }
        // 3. Eğer CompanyManager ise Token içindeki CompanyId'ye göre filtrele
        var companyIdStr = User.FindFirstValue("companyId");
        if (Guid.TryParse(companyIdStr, out Guid companyId))
        {
            var result = await _brandService.GetByCompanyIdAsync(companyId);
            return Ok(result);
        }

        // 4. Giriş yapmamış veya yetkisiz biri ise boş liste veya hata dönebilirsin
        return Ok(ApiResponse<IEnumerable<BrandDto>>.SuccessResult(new List<BrandDto>()));

    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _brandService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin,CompanyManager")] // Sadece Admin yeni marka ekleyebilir
    public async Task<IActionResult> Create(BrandCreateDto dto)
    {
        var result = await _brandService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Update(Guid id, BrandUpdateDto dto)
    {
        var result = await _brandService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _brandService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}