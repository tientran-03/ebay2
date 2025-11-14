using _30_TranVietTien.Models;
using _30_TranVietTien.Repositories;

namespace _30_TranVietTien.Services
{
    public class UserService : IUserService
    {
        private readonly CloneEbayDbContext _ctx;
        public UserService(CloneEbayDbContext ctx) { _ctx = ctx; }
        public async Task<User?> GetCurrentAsync(HttpContext ctx)
        {
            var id = ctx.Session.GetInt32("UserId");
            return id == null ? null : await _ctx.Users.FindAsync(id.Value);
        }
        public async Task UpdateProfileAsync(int userId, string username, string? avatarUrl)
        {
            var u = await _ctx.Users.FindAsync(userId);
            if (u == null) return;
            u.Username = username; u.AvatarUrl = avatarUrl;
            await _ctx.SaveChangesAsync();
        }
    }
}
