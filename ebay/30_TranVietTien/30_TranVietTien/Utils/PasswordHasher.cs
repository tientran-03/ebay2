using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

public static class PasswordHasher
{
    public static string Hash(string password, byte[]? salt = null)
    {
        salt ??= RandomNumberGenerator.GetBytes(16);
        var hashed = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100_000, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hashed)}";
    }

    public static bool Verify(string password, string stored)
    {
        var parts = stored.Split('.');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Hash(password, salt);
        return stored == hash;
    }
}
