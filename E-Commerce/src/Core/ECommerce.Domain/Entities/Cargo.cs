namespace ECommerce.Domain.Entities;

public class Cargo : BaseEntity
{
    public string Name { get; set; } = string.Empty; // Örn: Aras Kargo, Yurtiçi
    public string? TrackingUrlPrefix { get; set; } // Takip numarası sonuna eklenen URL
    public decimal BasePrice { get; set; } // Şirketin bu kargo için belirlediği standart fiyat

    // Her kargo ayarı bir şirkete özeldir
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
}