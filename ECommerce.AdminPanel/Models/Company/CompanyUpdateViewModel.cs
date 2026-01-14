using System.ComponentModel.DataAnnotations;

namespace ECommerce.AdminPanel.Models.Company;

public class CompanyUpdateViewModel
{
    public Guid Id { get; set; }

    [Required, MinLength(2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vergi numarası zorunludur.")]
    [Display(Name = "Vergi Numarası")]
    public string TaxNumber { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string FullAddress { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Phone { get; set; } = string.Empty;
    public bool Status { get; set; } = true;
    public bool IsApproved { get; set; } = false;
    
    //[Display(Name = "API Anahtarı (Sistem Tarafından Oluşturulur)")]
    //public string ApiKey { get; set; } = string.Empty;
}

