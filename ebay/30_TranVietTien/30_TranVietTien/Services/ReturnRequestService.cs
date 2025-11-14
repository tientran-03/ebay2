using _30_TranVietTien.Models;
using _30_TranVietTien.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _30_TranVietTien.Services
{
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly IReturnRequestRepository _repo;
        private readonly CloneEbayDbContext _context;

        public ReturnRequestService(IReturnRequestRepository repo, CloneEbayDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<bool> CreateRequestAsync(int orderId, int userId, string reason)
        {
            // 🔹 1. Kiểm tra đơn hàng có tồn tại & thuộc về user không
            var order = await _context.OrderTables
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.BuyerId == userId);

            if (order == null)
                throw new InvalidOperationException("❌ Order not found or not owned by this user.");

            // 🔹 2. Chỉ cho phép hoàn nếu đơn đã giao
            if (!string.Equals(order.Status, "Delivered", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("❌ Only delivered orders can be returned.");

            // 🔹 3. Kiểm tra trùng lặp yêu cầu hoàn hàng (mỗi đơn chỉ 1 lần)
            var existingRequest = await _context.ReturnRequests
                .FirstOrDefaultAsync(r => r.OrderId == orderId && r.UserId == userId && r.Status == "Pending");

            if (existingRequest != null)
                throw new InvalidOperationException("⚠️ A return request already exists for this order.");

            // 🔹 4. Tạo mới request
            var request = new ReturnRequest
            {
                OrderId = orderId,
                UserId = userId,
                Reason = reason,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(request);
            return true;
        }

        public async Task<bool> ApproveRequestAsync(int id)
        {
            var req = await _repo.GetByIdAsync(id);
            if (req == null || req.Status != "Pending")
                return false;

            req.Status = "Approved";
            await _repo.UpdateAsync(req);
            return true;
        }

        public async Task AutoApprovePendingRequestsAsync()
        {
            var pending = await _repo.GetPendingRequestsAsync();
            foreach (var req in pending)
            {
                if (req.CreatedAt.HasValue && (DateTime.Now - req.CreatedAt.Value).TotalHours > 12)
                {
                    req.Status = "AutoApproved";
                    await _repo.UpdateAsync(req);
                }
            }
        }

        public async Task<List<ReturnRequest>> GetUserRequestsAsync(int userId)
        {
            return await _repo.GetByUserAsync(userId);
        }
    }
}
