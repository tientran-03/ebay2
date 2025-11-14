using _30_TranVietTien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using _30_TranVietTien.Hubs;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Controllers
{
    public class ReviewController : Controller
    {
        private readonly CloneEbayDbContext _context;
        private readonly IHubContext<ReviewHub> _hub;

        public ReviewController(CloneEbayDbContext context, IHubContext<ReviewHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int productId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var hasDelivered = await _context.OrderItems
                .Include(o => o.Order)
                .AnyAsync(o =>
                    o.ProductId == productId &&
                    o.Order.BuyerId == userId &&
                    o.Order.Status == "Delivered"
                );

            if (!hasDelivered)
                return BadRequest("You can only review after receiving the item.");

            var review = new Review
            {
                ProductId = productId,
                ReviewerId = userId.Value,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var reviewer = await _context.Users.FindAsync(userId);

            await _hub.Clients.Group(productId.ToString()).SendAsync("ReceiveReview", new
            {
                reviewer = reviewer?.Username ?? "User",
                rating = rating,
                comment = comment,
                createdAt = review.CreatedAt?.ToString("yyyy-MM-dd HH:mm")
            });

            return RedirectToAction("Details", "Products", new { id = productId });
        }
    }
}
