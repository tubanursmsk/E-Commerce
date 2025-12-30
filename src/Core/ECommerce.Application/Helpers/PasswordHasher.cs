namespace ECommerce.Application.Helpers;

public static class PasswordHasher
{
    // Şifreyi hashler
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Şifreyi doğrular
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}