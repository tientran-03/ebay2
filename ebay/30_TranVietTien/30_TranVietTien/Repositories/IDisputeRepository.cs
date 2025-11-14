using _30_TranVietTien.Models;

namespace _30_TranVietTien.Repositories
{
    public interface IDisputeRepository
    {
        Task<IEnumerable<Dispute>> GetBySellerAsync(int sellerId);
        Task<Dispute?> GetByIdAsync(int id);
        Task UpdateAsync(Dispute dispute);
    }
}
