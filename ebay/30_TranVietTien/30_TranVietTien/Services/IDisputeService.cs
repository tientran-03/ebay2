using _30_TranVietTien.Models;

namespace _30_TranVietTien.Services
{
    public interface IDisputeService
    {
        Task<IEnumerable<Dispute>> GetBySellerAsync(int sellerId);
        Task<Dispute?> GetByIdAsync(int id);
        Task UpdateResolutionAsync(int id, string resolution, string status);
    }
}
