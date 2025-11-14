using _30_TranVietTien.Models;
using _30_TranVietTien.Repositories;

namespace _30_TranVietTien.Services
{
    public class DisputeService : IDisputeService
    {
        private readonly IDisputeRepository _repo;

        public DisputeService(IDisputeRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Dispute>> GetBySellerAsync(int sellerId)
            => await _repo.GetBySellerAsync(sellerId);

        public async Task<Dispute?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task UpdateResolutionAsync(int id, string resolution, string status)
        {
            var dispute = await _repo.GetByIdAsync(id);
            if (dispute == null) return;

            dispute.Resolution = resolution;
            dispute.Status = status;
            await _repo.UpdateAsync(dispute);
        }
    }
}
