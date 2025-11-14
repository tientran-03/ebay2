using System.Threading.Tasks;

namespace _30_TranVietTien.Services
{
    public interface IShippingService
    {
        Task CreateShippingAsync(int orderId);
    }
}
