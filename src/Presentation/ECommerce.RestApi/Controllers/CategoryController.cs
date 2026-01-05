using System.Security.Claims;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
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

    /*[HttpGet("List")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
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
        var result = await _categoryService.GetAllAsync();
        return Ok(result);
    }
     // 3. Eğer CompanyManager ise Token içindeki CompanyId'ye göre filtrele
    var companyIdStr = User.FindFirstValue("companyId");
    if (Guid.TryParse(companyIdStr, out Guid companyId))
    {
        var result = await _categoryService.GetByCompanyIdAsync(companyId);
        return Ok(result);
    }

    // 4. Giriş yapmamış veya yetkisiz biri ise boş liste veya hata dönebilirsin
    return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(new List<CategoryDto>()));
        
    }

    [HttpGet("GetById/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }


    [HttpGet("Company/{companyId}")]
    [Authorize(Policy = "CompanyIsolation")] // Başkasının kategorilerini görmeyi engeller
    public async Task<IActionResult> GetByCompany(Guid companyId)
    {
        var result = await _categoryService.GetByCompanyIdAsync(companyId);
        return Ok(result);
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin,CompanyManager")] 
    public async Task<IActionResult> Create(CategoryCreateDto dto)
    {
        var result = await _categoryService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPost("Update/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Update(Guid id, CategoryUpdateDto dto)
    {
        var result = await _categoryService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}