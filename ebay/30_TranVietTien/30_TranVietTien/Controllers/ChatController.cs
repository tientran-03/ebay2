using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;
using Microsoft.Extensions.Logging;

namespace _30_TranVietTien.Controllers
{
    public class ChatController : Controller
    {
        private readonly CloneEbayDbContext _ctx;
        private readonly ILogger<ChatController> _logger;

        public ChatController(CloneEbayDbContext ctx, ILogger<ChatController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public IActionResult Index()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }

        public async Task<IActionResult> Thread(int receiverId, int? productId)
        {
            int senderId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (senderId == 0) return RedirectToAction("Login", "Account");

            // Nếu buyer mở chat từ trang sản phẩm → tạo system message
            if (productId.HasValue)
            {
                var msg = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = "[system-product]",
                    ProductId = productId,
                    Timestamp = DateTime.Now
                };

                _ctx.Messages.Add(msg);
                await _ctx.SaveChangesAsync();
            }

            string room = $"{Math.Min(senderId, receiverId)}_{Math.Max(senderId, receiverId)}";

            var messages = await _ctx.Messages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            // lấy message chứa product
            var productMsg = messages.LastOrDefault(m => m.ProductId != null);

            if (productMsg != null)
            {
                var product = await _ctx.Products.FirstOrDefaultAsync(p => p.Id == productMsg.ProductId);

                if (product != null)
                {
                    ViewBag.ProductImage = product.Images;
                    ViewBag.ProductTitle = product.Title;
                    ViewBag.Price = product.Price;
                }
            }

            ViewBag.SenderId = senderId;
            ViewBag.ReceiverId = receiverId;
            ViewBag.Room = room;

            return View(messages);
        }



        [HttpPost]
        public async Task<IActionResult> SaveMessage(int senderId, int receiverId, string content)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning("Attempted to save empty message");
                    return BadRequest(new { success = false, message = "Nội dung tin nhắn trống" });
                }

                if (content.Length > 500)
                {
                    _logger.LogWarning("Message too long");
                    return BadRequest(new { success = false, message = "Tin nhắn quá dài" });
                }

                if (senderId <= 0 || receiverId <= 0)
                {
                    _logger.LogWarning($"Invalid user IDs - Sender: {senderId}, Receiver: {receiverId}");
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });
                }

                // Kiểm tra session
                int sessionUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (sessionUserId != senderId)
                {
                    _logger.LogWarning($"Session mismatch - Session: {sessionUserId}, Sender: {senderId}");
                    return Unauthorized(new { success = false, message = "Không có quyền gửi tin nhắn" });
                }

                // Kiểm tra người nhận có tồn tại không
                var receiverExists = await _ctx.Users.AnyAsync(u => u.Id == receiverId);
                if (!receiverExists)
                {
                    _logger.LogWarning($"Receiver not found: {receiverId}");
                    return NotFound(new { success = false, message = "Không tìm thấy người nhận" });
                }

                var message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content.Trim(),
                    Timestamp = DateTime.Now
                };

                _ctx.Messages.Add(message);
                await _ctx.SaveChangesAsync();

                _logger.LogInformation($"Message saved from {senderId} to {receiverId}");

                return Ok(new { success = true, messageId = message.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving message");
                return StatusCode(500, new { success = false, message = "Lỗi máy chủ" });
            }
        }

        public async Task<IActionResult> GetConversations()
        {
            try
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

                if (userId == 0)
                {
                    return Unauthorized(new { success = false, message = "Chưa đăng nhập" });
                }

                var conversations = await _ctx.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        UserName = g.FirstOrDefault()!.SenderId == userId 
                            ? g.FirstOrDefault()!.Receiver!.Username 
                            : g.FirstOrDefault()!.Sender!.Username,
                        LastMessage = g.OrderByDescending(m => m.Timestamp).FirstOrDefault()!.Content,
                        LastMessageTime = g.OrderByDescending(m => m.Timestamp).FirstOrDefault()!.Timestamp
                    })
                    .OrderByDescending(c => c.LastMessageTime)
                    .ToListAsync();

                return Ok(new { success = true, conversations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversations");
                return StatusCode(500, new { success = false, message = "Lỗi máy chủ" });
            }
        }
    }
}
