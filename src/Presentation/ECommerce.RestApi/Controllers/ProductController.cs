using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("List")]
    [AllowAnonymous] // Ürünleri giriş yapmayanlar da görebilsin
    public async Task<IActionResult> GetAll()
    {
        var result = await _productService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _productService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("Search")]
    [AllowAnonymous] // Arama yapmak için giriş zorunluluğu olmasın
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        var result = await _productService.SearchAsync(keyword);
        return Ok(result);
    }

    [HttpGet("List/{companyId}")]
    [Authorize(Policy = "CompanyIsolation")] // Şirket bazlı koruma
    public async Task<IActionResult> GetByCompany(Guid companyId)
    {
        var result = await _productService.GetByCompanyIdAsync(companyId);
        return Ok(result);
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin")] // Sadece Admin ürün ekleyebilir
    //[ApiKey] // X-Api-Key Header kontrolü
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Admin")]
    [Authorize(Policy = "CompanyIsolation")] // Başkasının ürününü güncellemeyi engeller
    [ApiKey] // X-Api-Key Header kontrolü
    public async Task<IActionResult> Update(Guid id, ProductUpdateDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin")]
    [Authorize(Policy = "CompanyIsolation")]
    [ApiKey]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}