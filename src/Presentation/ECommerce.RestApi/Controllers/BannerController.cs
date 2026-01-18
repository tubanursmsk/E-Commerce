using System.Security.Claims;
using ECommerce.Application.DTOs.Banner;
using ECommerce.Application.Interfaces;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
//[ApiKey]
public class BannerController : ControllerBase
{
    private readonly IBannerService _bannerService;

    public BannerController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    /*[HttpGet("List")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var companyIdStr = User.FindFirstValue("companyId");
        Guid? companyId = string.IsNullOrEmpty(companyIdStr) ? null : Guid.Parse(companyIdStr);

        return Ok(await _bannerService.GetAllAsync(companyId, role));
    }

    //*Siz bu metoda [AllowAnonymous] verdiğiniz için ve Angular tarafında henüz bir Token göndermediğimiz için, 
    //*User.FindFirstValue metodları her zaman null döner.
        Muhtemelen BannerService.GetAllAsync metodunun içinde şöyle bir mantık var:
        Eğer role null ise veya companyId null ise sorguyu boş dön (veya yanlış bir filtreleme yap).
        ✅ Kesin Çözüm: BannerService ve Controller Güncellemesi
        Banner'lar genellikle tüm kullanıcılar (giriş yapmayanlar dahil) tarafından görülmesi gereken verilerdir. 
        Bu yüzden filtreleme mantığını "eğer Admin değilse sadece aktifleri getir" şeklinde sadeleştirmeliyiz.
        Giriş yapmayan bir kullanıcı için role ve companyId göndermeye çalışmak sorguyu bozuyor. Burayı sadece
         servise yetki kısıtı olmadan çağrı yapacak hale getirin:
    
    */

    [HttpGet("List")]
    [AllowAnonymous] // Giriş yapmayan herkes görebilsin
    public async Task<IActionResult> GetAll()
    {
        // Giriş yapılmadığında bunlar null dönecek ve servis her şeyi getirecek
        var role = User.FindFirstValue(ClaimTypes.Role);
        var companyIdStr = User.FindFirstValue("companyId");
        Guid? companyId = string.IsNullOrEmpty(companyIdStr) ? null : Guid.Parse(companyIdStr);

        return Ok(await _bannerService.GetAllAsync(companyId, role));
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin,CompanyManager")] // Sadece Admin banner ekleyebilir
    public async Task<IActionResult> Create(BannerCreateDto dto) => Ok(await _bannerService.CreateAsync(dto));


    [HttpGet("GetById/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _bannerService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Update(Guid id, BannerUpdateDto dto) => Ok(await _bannerService.UpdateAsync(id, dto));

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> Delete(Guid id) => Ok(await _bannerService.DeleteAsync(id));
}