using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.RestApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private const string APIKEYNAME = "X-Api-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 1. Header'da X-Api-Key var mı?
        if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            context.Result = new ContentResult() { StatusCode = 401, Content = "API Key eksik." };
            return;
        }

        // 2. Veritabanındaki Company API Key'leri ile eşleşiyor mu?
        var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var companies = await unitOfWork.Companies.FindAsync(c => c.ApiKey == extractedApiKey.ToString() && c.IsApproved);

        if (!companies.Any())
        {
            context.Result = new ContentResult() { StatusCode = 403, Content = "Geçersiz veya onaylanmamış API Key." };
            return;
        }

        // 3. Başarılıysa Şirket ID'sini Context'e ekleyelim (Servislerde kullanmak için)
        context.HttpContext.Items["CompanyId"] = companies.First().Id;

        await next();
    }
}


/*

Rol Tabanlı Güvenlik Teyidi
Evet, şu ana kadar attığımız adımlar altyapıyı doğru kurdu ancak tam olarak "aktif" hale gelmesi için küçük bir dokunuş gerekiyor:

Entity Seviyesi: User tablosunda Role alanımız var (Doğru).

JWT Seviyesi: JwtTokenHelper içinde rolleri Claim olarak ekledik (Doğru).

Eksik Olan: Controller'ların başına [Authorize(Roles = "Admin")] gibi kısıtlamaları eklemek. Bu, altyapının meyvesini topladığımız yer olacak.

X-Api-Key (Şirket Bazlı Güvenlik) Yapılandırması
Yönergedeki bu madde, sistemin sadece "kim olduğunu" (JWT) değil, "hangi şirkete hizmet ettiğini" (API Key) bilmesini sağlar. Bunu bir Action Filter ile yapmak en profesyonel yoldur.


Kullanıcı
   |
   | ---> [JWT Token ile kimlik doğrulama]
   |          - Kullanıcı giriş yapmış mı?
   |          - Token geçerli mi, süresi dolmamış mı?
   |          - Claims (roller, izinler) doğru mu?
   |
   v
API Sunucusu
   |
   | ---> [API Key kontrolü]
   |          - Header'da X-Api-Key var mı?
   |          - Bu key Companies tablosunda kayıtlı mı?
   |          - Şirket onaylı mı (IsApproved)?
   |
   v
Yetkilendirme Katmanı
   |
   | ---> [Başarılıysa]
   |          - HttpContext.Items["CompanyId"] = ilgili şirket Id
   |          - Action çalıştırılır
   |
   | ---> [Başarısızsa]
   |          - 401 Unauthorized (JWT yok/geçersiz)
   |          - 403 Forbidden (API Key yok/geçersiz/onaysız)
   |
   v
Controller Action (ör. ProductController)
   |
   | ---> Servisler, Repository, DB işlemleri
   |
   v
Response (JSON / HTTP Status)

using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.RestApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private const string APIKEYNAME = "X-Api-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 1. Header'da X-Api-Key var mı?
        if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            context.Result = new ContentResult() { StatusCode = 401, Content = "API Key eksik." };
            return;
        }

        // 2. Veritabanındaki Company API Key'leri ile eşleşiyor mu?
        var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var companies = await unitOfWork.Companies.FindAsync(c => c.ApiKey == extractedApiKey.ToString() && c.IsApproved);

        if (!companies.Any())
        {
            context.Result = new ContentResult() { StatusCode = 403, Content = "Geçersiz veya onaylanmamış API Key." };
            return;
        }

        // 3. Başarılıysa Şirket ID'sini Context'e ekleyelim (Servislerde kullanmak için)
        context.HttpContext.Items["CompanyId"] = companies.First().Id;

        await next();
    }
}


/*

Rol Tabanlı Güvenlik Teyidi
Evet, şu ana kadar attığımız adımlar altyapıyı doğru kurdu ancak tam olarak "aktif" hale gelmesi için küçük bir dokunuş gerekiyor:

Entity Seviyesi: User tablosunda Role alanımız var (Doğru).

JWT Seviyesi: JwtTokenHelper içinde rolleri Claim olarak ekledik (Doğru).

Eksik Olan: Controller'ların başına [Authorize(Roles = "Admin")] gibi kısıtlamaları eklemek. Bu, altyapının meyvesini topladığımız yer olacak.

X-Api-Key (Şirket Bazlı Güvenlik) Yapılandırması
Yönergedeki bu madde, sistemin sadece "kim olduğunu" (JWT) değil, "hangi şirkete hizmet ettiğini" (API Key) bilmesini sağlar. Bunu bir Action Filter ile yapmak en profesyonel yoldur.


Kullanıcı
   |
   | ---> [JWT Token ile kimlik doğrulama]
   |          - Kullanıcı giriş yapmış mı?
   |          - Token geçerli mi, süresi dolmamış mı?
   |          - Claims (roller, izinler) doğru mu?
   |
   v
API Sunucusu
   |
   | ---> [API Key kontrolü]
   |          - Header'da X-Api-Key var mı?
   |          - Bu key Companies tablosunda kayıtlı mı?
   |          - Şirket onaylı mı (IsApproved)?
   |
   v
Yetkilendirme Katmanı
   |
   | ---> [Başarılıysa]
   |          - HttpContext.Items["CompanyId"] = ilgili şirket Id
   |          - Action çalıştırılır
   |
   | ---> [Başarısızsa]
   |          - 401 Unauthorized (JWT yok/geçersiz)
   |          - 403 Forbidden (API Key yok/geçersiz/onaysız)
   |
   v
Controller Action (ör. ProductController)
   |
   | ---> Servisler, Repository, DB işlemleri
   |
   v
Response (JSON / HTTP Status)


Controller'a uygulayalım. Örneğin sadece onaylı şirketlerin ürün ekleyebilmesini istiyorsak:

C#

[ApiController]
[Route("api/[controller]")]
[ApiKey] // Artık bu Controller'daki her istek X-Api-Key header'ı istemek zorunda
public class ProductController : ControllerBase
{
    ...
}




*/

