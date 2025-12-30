namespace ECommerce.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,      // Beklemede
    Processing = 2,   // Hazırlanıyor
    Shipped = 3,      // Kargolandı
    Completed = 4,    // Tamamlandı
    Cancelled = 5     // İptal Edildi
}