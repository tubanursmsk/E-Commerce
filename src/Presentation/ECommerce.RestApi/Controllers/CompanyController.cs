using ECommerce.Application.DTOs.Company;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    // Herkes şirket listesini görebilir mi? (Yönergeye göre Admin görebilmeli)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _companyService.GetAllAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // Yeni şirket kaydı (Mağaza Başvurusu)
    [HttpPost("Create")]
    public async Task<IActionResult> Create(CompanyCreateDto dto)
    {
        var result = await _companyService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("Update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, CompanyUpdateDto dto)
    {
        var result = await _companyService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }


    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _companyService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // Şirket Onaylama (Sadece Admin yapabilir)
    [HttpPatch("Approve/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var result = await _companyService.ApproveCompanyAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}