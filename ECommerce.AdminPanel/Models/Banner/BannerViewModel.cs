using System.ComponentModel.DataAnnotations;

namespace ECommerce.AdminPanel.Models.Banner;
public class BannerViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? TargetUrl { get; set; }
    public int Order { get; set; }
    public bool Status { get; set; }
}