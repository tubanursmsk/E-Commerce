namespace ECommerce.AdminPanel.Models.Banner;
using System.ComponentModel.DataAnnotations;

public class CreateBannerViewModel
{
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

/*
🔍 Mülakat İpucu: Neden URL Tercih Edildi?
Mülakatta bu kısım sorulursa şu cevabı verebilirsin:

"Sistem mimarisini Cloud-Native yaklaşıma hazırlamak adına şimdilik URL tabanlı ilerledim. 
Bu sayede resimler CDN (Content Delivery Network) veya harici depolama birimlerinden (S3, Blob) 
çekilerek sunucu üzerindeki I/O yükü minimize ediliyor. Gelecek aşamada entegre bir Image Service
 ile hem upload hem de URL desteği sunan hibrit bir yapı kurmayı planlıyorum."

*/