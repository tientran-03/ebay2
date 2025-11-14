using _30_TranVietTien.Models;

public interface IInventoryService
{
    Task UpdateStockAsync(int productId, int quantity);
}
