using System.Threading.Tasks;

namespace _30_TranVietTien.Services
{
    public interface IPaymentGateway
    {
        Task<bool> PayAsync(decimal amount, string method);
    }

    public class PaymentGateway : IPaymentGateway
    {
        public async Task<bool> PayAsync(decimal amount, string method)
        {
            await Task.Delay(100); // Giả lập gọi API thanh toán
            return method == "PayPal" || method == "COD";
        }
    }
}
