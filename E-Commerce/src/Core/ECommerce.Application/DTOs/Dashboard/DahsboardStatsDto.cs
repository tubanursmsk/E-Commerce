namespace ECommerce.Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public decimal MonthlyTotalSales { get; set; } // Aylık Toplam Satış
    public int PendingOrdersCount { get; set; }    // Bekleyen Sipariş Sayısı
    public int CriticalStockCount { get; set; }     // Kritik Stoktaki Ürün Sayısı
    public int TotalCustomersCount { get; set; }    // Toplam Müşteri Sayısı
    
    public List<TopProductDto> TopProducts { get; set; } = new(); // Grafik için En Çok Satanlar
}

public class TopProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public int TotalSold { get; set; } // Satış Adedi
}