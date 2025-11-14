using Microsoft.AspNetCore.Mvc;

namespace _30_TranVietTien.Services
{
    public class NotificationService : INotificationService
    {
        // demo in-memory; thực tế dùng table riêng
        private static readonly Dictionary<int, List<string>> _mem = new();
        public Task CreateAsync(int userId, string message)
        {
            if (!_mem.ContainsKey(userId)) _mem[userId] = new();
            _mem[userId].Add($"{DateTime.Now:HH:mm} - {message}");
            return Task.CompletedTask;
        }
        public Task<List<string>> GetAsync(int userId)
            => Task.FromResult(_mem.ContainsKey(userId) ? _mem[userId] : new List<string>());
    }
}
