using System.Security.Claims;
using ECommerce.Application.DTOs.Cargo;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
//[ApiKey]
[Authorize]
public class CargoController : ControllerBase
{
    private readonly ICargoService _cargoService;
    public CargoController(ICargoService cargoService)
    {
        _cargoService = cargoService;
    }

    [HttpGet]
    [AllowAnonymous] // Kargo firmalarını herkes görebilmeli
    public async Task<IActionResult> GetAll()
    {
        // 1. Kullanıcının rolünü alalım
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        // 2. Eğer kullanıcı Admin ise tüm ürünleri getir
        if (userRole == "Admin")
        {
            var result = await _cargoService.GetAllAsync();
            return Ok(result);
        }
        // 3. Eğer CompanyManager ise Token içindeki CompanyId'ye göre filtrele
        var companyIdStr = User.FindFirstValue("companyId");
        if (Guid.TryParse(companyIdStr, out Guid companyId))
        {
            var result = await _cargoService.GetByCompanyIdAsync(companyId);
            return Ok(result);
        }
        // 4. Giriş yapmamış veya yetkisiz biri ise boş liste veya hata dönebilirsin
        return Ok(ApiResponse<IEnumerable<CargoDto>>.SuccessResult(new List<CargoDto>()));
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin,CompanyManager")] // Sadece sistem yöneticisi kargo ekleyebilir
    public async Task<IActionResult> Create(CargoCreateDto dto) => Ok(await _cargoService.CreateAsync(dto));


    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _cargoService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Update(Guid id, CargoUpdateDto dto) => Ok(await _cargoService.UpdateAsync(id, dto));

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Delete(Guid id) => Ok(await _cargoService.DeleteAsync(id));
}
