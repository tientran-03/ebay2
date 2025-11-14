using _30_TranVietTien.Models;
using Microsoft.AspNetCore.Mvc;

namespace _30_TranVietTien.Services
{
    public class ReviewService : IReviewService
    {
        private readonly CloneEbayDbContext _ctx;
        public ReviewService(CloneEbayDbContext ctx) { _ctx = ctx; }
        public async Task AddAsync(int userId, int productId, int rating, string? comment)
        {
            var r = new Review { ReviewerId = userId, ProductId = productId, Rating = rating, Comment = comment, CreatedAt = DateTime.UtcNow };
            _ctx.Reviews.Add(r);
            await _ctx.SaveChangesAsync();
        }
    }
}
