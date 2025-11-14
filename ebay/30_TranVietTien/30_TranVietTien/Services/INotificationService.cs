using Microsoft.AspNetCore.Mvc;

namespace _30_TranVietTien.Services
{
    public interface INotificationService
    {
        Task CreateAsync(int userId, string message);
        Task<List<string>> GetAsync(int userId);
    }
}
