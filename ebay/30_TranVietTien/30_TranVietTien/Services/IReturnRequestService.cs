using _30_TranVietTien.Models;
using System.Threading.Tasks;

namespace _30_TranVietTien.Services
{
    public interface IReturnRequestService
    {
        Task<bool> CreateRequestAsync(int orderId, int userId, string reason);
        Task<bool> ApproveRequestAsync(int id);
        Task AutoApprovePendingRequestsAsync();
        Task<List<ReturnRequest>> GetUserRequestsAsync(int userId);
    }

}
