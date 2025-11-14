using Microsoft.AspNetCore.Mvc;

namespace _30_TranVietTien.Services
{
    public interface IReviewService
    {
        Task AddAsync(int userId, int productId, int rating, string? comment);
    }
}
