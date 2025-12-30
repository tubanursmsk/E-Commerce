namespace ECommerce.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // Role-Based Auth için
    public string Role { get; set; } = "Customer"; // Admin, CompanyManager, Customer

    // Eğer kullanıcı bir şirkete bağlıysa (Yönetici ise)
    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }
}