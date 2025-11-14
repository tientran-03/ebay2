using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CloneEbayDbContext _context;
        public OrderRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<OrderTable> CreateOrderAsync(OrderTable order, List<OrderItem> items)
        {
            _context.OrderTables.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                item.OrderId = order.Id;
                _context.OrderItems.Add(item);
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateOrderAsync(OrderTable order)
        {
            _context.OrderTables.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderTable?> GetOrderByIdAsync(int id)
        {
            return await _context.OrderTables
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .Include(o => o.Payments)
                .Include(o => o.ShippingInfos)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<OrderTable>> GetOrdersByUserAsync(int userId)
        {
            return await _context.OrderTables
                .Include(o => o.Payments)
                .Include(o => o.ShippingInfos)
                .Where(o => o.BuyerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<OrderTable>> GetOrdersBySellerAsync(int sellerId)
       => await _context.OrderTables
           .Include(o => o.OrderItems).ThenInclude(i => i.Product)
           .Where(o => o.OrderItems.Any(i => i.Product.SellerId == sellerId))
           .ToListAsync();

        public async Task<OrderTable?> GetByIdAsync(int id)
            => await _context.OrderTables
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task UpdateAsync(OrderTable order)
        {
            _context.OrderTables.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
