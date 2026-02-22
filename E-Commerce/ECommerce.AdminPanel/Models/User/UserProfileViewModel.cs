namespace ECommerce.AdminPanel.Models.User;
using System.ComponentModel.DataAnnotations;

public class UserProfileViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
    [Display(Name = "Adınız")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad alanı boş bırakılamaz.")]
    [Display(Name = "Soyadınız")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty; // Rol değiştirilemez, sadece gösterilir
}