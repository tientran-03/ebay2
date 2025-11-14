using _30_TranVietTien.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _30_TranVietTien.Repositories
{
    public interface IOrderRepository
    {
        Task<OrderTable> CreateOrderAsync(OrderTable order, List<OrderItem> items);
        Task UpdateOrderAsync(OrderTable order);
        Task<OrderTable?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderTable>> GetOrdersByUserAsync(int userId);
        Task<IEnumerable<OrderTable>> GetOrdersBySellerAsync(int sellerId);
        Task<OrderTable?> GetByIdAsync(int id);
        Task UpdateAsync(OrderTable order);
    }
}
