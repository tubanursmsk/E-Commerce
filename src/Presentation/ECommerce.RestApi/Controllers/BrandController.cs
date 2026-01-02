using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.Interfaces;
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

    [HttpGet("List")]
    [AllowAnonymous] // Markaları herkes görebilsin (Katalog amaçlı)
    public async Task<IActionResult> GetAll()
    {
        var result = await _brandService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _brandService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin")] // Sadece Admin yeni marka ekleyebilir
    public async Task<IActionResult> Create(BrandCreateDto dto)
    {
        var result = await _brandService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, BrandUpdateDto dto)
    {
        var result = await _brandService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _brandService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}