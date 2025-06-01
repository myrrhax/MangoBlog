using Application.Abstractions;

namespace Infrastructure.Implementation;

internal class PasswordHasherImpl : IPasswordHasher
{
    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

    public bool VerifyPassword(string password, string hash)
        => BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
}
