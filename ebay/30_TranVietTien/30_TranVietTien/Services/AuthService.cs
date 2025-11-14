using System.Security.Cryptography;
using System.Text;

namespace _30_TranVietTien.Services
{
    public class AuthService : IAuthService
    {
        // Hash password đơn giản bằng SHA256
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // So sánh hash khi đăng nhập
        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var hashed = HashPassword(enteredPassword);
            return hashed == storedHash;
        }
    }
}
