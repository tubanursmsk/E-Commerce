using System.Security.Claims;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
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


    [HttpGet("ListAll")] //Ürünleri giriş yapmayanlarda görebilsin diye olan metod
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProduct()
    {
        var result = await _productService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("List")]
    public async Task<IActionResult> GetAll()
    {
        // 1. Kullanıcının rolünü alalım
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        // 2. Eğer kullanıcı Admin ise tüm ürünleri getir
        if (userRole == "Admin")
        {
            var result = await _productService.GetAllAsync();
            return Ok(result);
        }

        // 3. Eğer CompanyManager ise Token içindeki CompanyId'ye göre filtrele
        var companyIdStr = User.FindFirstValue("companyId");
        if (Guid.TryParse(companyIdStr, out Guid companyId))
        {
            var result = await _productService.GetByCompanyIdAsync(companyId);
            return Ok(result);
        }

        // 4. Giriş yapmamış veya yetkisiz biri ise boş liste veya hata dönebilirsin
        return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResult(new List<ProductDto>()));
    }


    [HttpGet("GetById/{id}")]
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
    [Authorize(Roles = "Admin,CompanyManager")] // Sadece Admin ürün ekleyebilir
    //[ApiKey] // X-Api-Key Header kontrolü
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("Update/{id}")]  // PUT da olabilir hata alırsan put ile dene
    [Authorize(Roles = "Admin,CompanyManager")]
    [Authorize(Policy = "CompanyIsolation")] // Başkasının ürününü güncellemeyi engeller
    //[ApiKey] // X-Api-Key Header kontrolü
    public async Task<IActionResult> Update(Guid id, ProductUpdateDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    [Authorize(Policy = "CompanyIsolation")]
    //[ApiKey]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}