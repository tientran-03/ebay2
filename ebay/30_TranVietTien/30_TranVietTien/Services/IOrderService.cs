// Services/IOrderService.cs
using _30_TranVietTien.Models;

namespace _30_TranVietTien.Services
{
    public interface IOrderService
    {
        Task<OrderTable> CreateOrderAsync(int userId, List<OrderItem> items, decimal total, int addressId);
        Task<bool> CompletePaymentAsync(int orderId, int userId, decimal amount, string method);
        Task<List<OrderTable>> GetUserOrdersAsync(int userId);
        Task<OrderTable?> GetOrderAsync(int id);
        Task<IEnumerable<OrderTable>> GetOrdersBySellerAsync(int sellerId);
        Task UpdateStatusAsync(int orderId, string status);
    }
}
