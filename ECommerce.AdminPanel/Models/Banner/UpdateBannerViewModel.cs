namespace ECommerce.AdminPanel.Models.Banner;
using System.ComponentModel.DataAnnotations;

public class UpdateBannerViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Banner başlığı zorunludur.")]
    [Display(Name = "Banner Başlığı")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Resim URL adresi zorunludur.")]
    [Display(Name = "Resim URL")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Hedef Bağlantı (Target URL)")]
    public string? TargetUrl { get; set; }

    [Required(ErrorMessage = "Görüntülenme sırası zorunludur.")]
    [Display(Name = "Sıralama")]
    public int Order { get; set; } = 1;

    [Display(Name = "Durum (Aktif/Pasif)")]
    public bool Status { get; set; } = true;
    public Guid CompanyId { get; set; }
}

