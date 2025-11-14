using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly CloneEbayDbContext _context;
        public ReportRepository(CloneEbayDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<IEnumerable<OrderTable>> GetOrdersBySellerAsync(int sellerId)
        {
            return await _context.OrderTables
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(i => i.Product.SellerId == sellerId))
                .ToListAsync();
        }
    }
}
