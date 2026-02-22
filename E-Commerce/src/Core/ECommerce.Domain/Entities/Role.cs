namespace ECommerce.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty; // Admin, CompanyManager, Customer
    public string? Description { get; set; }

    // Kullanıcılar ile çok-a-çok ilişki kurmak için (Opsiyonel: Basit yapıda string de tutulabilir)
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}