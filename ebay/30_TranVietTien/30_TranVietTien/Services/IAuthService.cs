using _30_TranVietTien.Models;
using Microsoft.AspNetCore.Mvc;
namespace _30_TranVietTien.Services
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string enteredPassword, string storedHash);
    }
}
