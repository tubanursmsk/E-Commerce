using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Company
{
    public class CompanyApiKeyDto
    {
        public Guid CompanyId { get; set; }
        public string ApiKey { get; set; } = string.Empty;
    }

    public class CompanyRegenerateApiKeyDto
    {
        [Required]
        public Guid CompanyId { get; set; }
    }
}

/*
Bu yönerge “şirket bazlı API Key desteği” istediği için en temiz yaklaşım, ApiKey’yi CRUD’dan ayırmak:

Admin: “API Key gör / yenile”

Company Manager: “kendi ApiKey’ini gör (opsiyonel)”
*/