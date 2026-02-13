using ECommerce.Application.DTOs.Company;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
    [HttpGet("List")]
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

    [HttpGet("GetById/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Güvenlik: Manager ise sadece kendi ID'sini isteyebilir
    if (User.IsInRole("CompanyManager") && id.ToString() != User.FindFirstValue("companyId"))
        return Forbid();

    return Ok(await _companyService.GetByIdAsync(id));
    }

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Update(Guid id, CompanyUpdateDto dto)
    {
        // Güvenlik Kontrolü: Eğer Admin değilse, sadece kendi şirketini güncelleyebilir
        if (!User.IsInRole("Admin"))
        {
            var userCompanyId = User.FindFirstValue("companyId");
            if (id.ToString() != userCompanyId) return Forbid();
        }
        
        return Ok(await _companyService.UpdateAsync(id, dto));
    }


    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin")]
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


//Silme işleminin (Soft Delete) başarılı olması çok iyi. Veritabanına gidip baktığında
// IsDeleted alanının 1 (true) olduğunu, ancak verinin hala orada durduğunu görebilirsin. 
//Bu, e-ticaret sistemlerinde veri kaybını önlemek için profesyonel bir yaklaşımdır.