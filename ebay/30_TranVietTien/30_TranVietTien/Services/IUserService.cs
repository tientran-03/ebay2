using _30_TranVietTien.Models;

namespace _30_TranVietTien.Services
{
    public interface IUserService
    {
        Task<User?> GetCurrentAsync(HttpContext ctx);
        Task UpdateProfileAsync(int userId, string username, string? avatarUrl);
    }
}
