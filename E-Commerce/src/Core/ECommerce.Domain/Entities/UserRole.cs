namespace ECommerce.Domain.Entities;

public class UserRole : BaseEntity
{
    // İlişkili Kullanıcının Id'si
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // İlişkili Rolün Id'si
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

