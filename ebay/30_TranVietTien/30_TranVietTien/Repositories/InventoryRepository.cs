using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;

public class InventoryRepository : IInventoryRepository
{
    private readonly CloneEbayDbContext _ctx;
    public InventoryRepository(CloneEbayDbContext ctx) => _ctx = ctx;

    public async Task<Inventory?> GetByProductIdAsync(int productId)
        => await _ctx.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);

    public async Task UpdateAsync(Inventory inv)
    {
        _ctx.Inventories.Update(inv);
        await _ctx.SaveChangesAsync();
    }

    public async Task AddAsync(Inventory inv)
    {
        await _ctx.Inventories.AddAsync(inv);
        await _ctx.SaveChangesAsync();
    }
}
