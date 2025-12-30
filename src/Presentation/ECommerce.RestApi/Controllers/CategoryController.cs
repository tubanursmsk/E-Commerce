using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
//[ApiKey] // Her istekte X-Api-Key zorunlu
[Authorize] // Her istekte geçerli JWT Token zorunlu
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("List")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("Company/{companyId}")]
    [Authorize(Policy = "CompanyIsolation")] // Başkasının kategorilerini görmeyi engeller
    public async Task<IActionResult> GetByCompany(Guid companyId)
    {
        var result = await _categoryService.GetByCompanyIdAsync(companyId);
        return Ok(result);
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin")] // Sadece Admin kategori ekleyebilir
    public async Task<IActionResult> Create(CategoryCreateDto dto)
    {
        var result = await _categoryService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPost("Update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, CategoryUpdateDto dto)
    {
        var result = await _categoryService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}