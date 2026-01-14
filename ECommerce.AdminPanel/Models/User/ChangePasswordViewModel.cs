namespace ECommerce.AdminPanel.Models.User;
using System.ComponentModel.DataAnnotations;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Mevcut şifrenizi giriniz.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mevcut Şifre")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre (Tekrar)")]
    public string ConfirmPassword { get; set; } = string.Empty;
}