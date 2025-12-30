namespace ECommerce.Domain.Entities;


public class Request : BaseEntity
{
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Feedback { get; set; } // Adminin verdiği cevap
    public bool IsResolved { get; set; } = false; // Çözüldü mü?

    // Talebi oluşturan kullanıcı
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Hangi şirkete yönelik bir talep olduğu
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}