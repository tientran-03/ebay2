// Services/OrderService.cs
using _30_TranVietTien.Models;
using _30_TranVietTien.Repositories;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Services
{
    public class OrderService : IOrderService
    {
        private readonly CloneEbayDbContext _context;
        private readonly IPaymentGateway _pg;
        private readonly IOrderRepository _repo;

        public OrderService(CloneEbayDbContext context, IPaymentGateway pg)
        {
            _context = context;
            _pg = pg;
        }

        public async Task<OrderTable> CreateOrderAsync(int userId, List<OrderItem> items, decimal total, int addressId)
        {
            var order = new OrderTable
            {
                BuyerId = userId,
                AddressId = addressId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = total,
                Status = "Pending"
            };
            _context.OrderTables.Add(order);
            await _context.SaveChangesAsync();

            foreach (var it in items)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = it.ProductId,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice
                });
            }
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> CompletePaymentAsync(int orderId, int userId, decimal amount, string method)
        {
            var ok = await _pg.PayAsync(amount, method);
            _context.Payments.Add(new Payment
            {
                OrderId = orderId,
                UserId = userId,
                Amount = amount,
                Method = method,
                Status = ok ? "Success" : "Failed",
                PaidAt = DateTime.UtcNow
            });

            var order = await _context.OrderTables.FirstAsync(o => o.Id == orderId);
            order.Status = ok ? "Paid" : "PaymentFailed";
            await _context.SaveChangesAsync();
            return ok;
        }

        public Task<List<OrderTable>> GetUserOrdersAsync(int userId)
        {
            return _context.OrderTables
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .Include(o => o.Payments)
                .Where(o => o.BuyerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public Task<OrderTable?> GetOrderAsync(int id)
        {
            return _context.OrderTables
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .Include(o => o.Payments)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<IEnumerable<OrderTable>> GetOrdersBySellerAsync(int sellerId)
      => await _repo.GetOrdersBySellerAsync(sellerId);

        public async Task UpdateStatusAsync(int orderId, string status)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _repo.UpdateAsync(order);
            }
        }
    }
}
