using _30_TranVietTien.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _30_TranVietTien.Repositories
{
    public interface IReturnRequestRepository
    {
        Task AddAsync(ReturnRequest request);
        Task<ReturnRequest?> GetByIdAsync(int id);
        Task<List<ReturnRequest>> GetPendingRequestsAsync();
        Task<List<ReturnRequest>> GetByUserAsync(int userId);
        Task UpdateAsync(ReturnRequest request);
    }
}
