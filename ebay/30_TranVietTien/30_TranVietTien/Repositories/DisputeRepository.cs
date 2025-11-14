using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Repositories
{
    public class DisputeRepository : IDisputeRepository
    {
        private readonly CloneEbayDbContext _ctx;
        public DisputeRepository(CloneEbayDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Dispute>> GetBySellerAsync(int sellerId)
        {
            return await _ctx.Disputes
                .Include(d => d.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(d => d.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(i => i.Product)
                .Where(d => d.Order!.OrderItems.Any(i => i.Product!.SellerId == sellerId))
                .OrderByDescending(d => d.Id)
                .ToListAsync();
        }

        public async Task<Dispute?> GetByIdAsync(int id)
            => await _ctx.Disputes.Include(d => d.Order).FirstOrDefaultAsync(d => d.Id == id);

        public async Task UpdateAsync(Dispute dispute)
        {
            _ctx.Disputes.Update(dispute);
            await _ctx.SaveChangesAsync();
        }
    }
}
