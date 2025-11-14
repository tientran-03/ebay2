using _30_TranVietTien.Models;

namespace _30_TranVietTien.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<OrderTable>> GetOrdersBySellerAsync(int sellerId);
    }
}
