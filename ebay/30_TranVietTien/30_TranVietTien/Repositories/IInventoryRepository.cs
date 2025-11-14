using _30_TranVietTien.Models;

public interface IInventoryRepository
{
    Task<Inventory?> GetByProductIdAsync(int productId);
    Task UpdateAsync(Inventory inv);
    Task AddAsync(Inventory inv);
}
