using ECommerce.Application.DTOs.Cargo;
using ECommerce.Application.Interfaces;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiKey]
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
    public async Task<IActionResult> GetAll() => Ok(await _cargoService.GetAllAsync());

    [HttpPost("Create")]
    [Authorize(Roles = "Admin")] // Sadece sistem yöneticisi kargo ekleyebilir
    public async Task<IActionResult> Create(CargoCreateDto dto) => Ok(await _cargoService.CreateAsync(dto));

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, CargoUpdateDto dto) => Ok(await _cargoService.UpdateAsync(id, dto));

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id) => Ok(await _cargoService.DeleteAsync(id));
}