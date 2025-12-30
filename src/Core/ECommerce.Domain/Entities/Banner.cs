namespace ECommerce.Domain.Entities;

public class Banner : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? TargetUrl { get; set; } // Banner'a tıklandığında gidilecek link
    public int Order { get; set; } // Görüntülenme sırası

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}