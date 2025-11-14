using _30_TranVietTien.Models;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repo;

    public InventoryService(IInventoryRepository repo)
    {
        _repo = repo;
    }

    public async Task UpdateStockAsync(int productId, int quantity)
    {
        var inv = await _repo.GetByProductIdAsync(productId);
        if (inv != null)
        {
            inv.Quantity = quantity;
            inv.LastUpdated = DateTime.Now;
            await _repo.UpdateAsync(inv);
        }
        else
        {
            await _repo.AddAsync(new Inventory
            {
                ProductId = productId,
                Quantity = quantity,
                LastUpdated = DateTime.Now
            });
        }
    }
}
