using _30_TranVietTien.Models;

namespace _30_TranVietTien.Services
{
    public class ShippingService : IShippingService
    {
        private readonly CloneEbayDbContext _context;
        public ShippingService(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task CreateShippingAsync(int orderId)
        {
            var shipping = new ShippingInfo
            {
                OrderId = orderId,
                Carrier = "eBay Express",
                TrackingNumber = "EB" + new Random().Next(100000, 999999),
                Status = "Processing",
                EstimatedArrival = DateTime.UtcNow.AddDays(3)
            };

            _context.ShippingInfos.Add(shipping);
            await _context.SaveChangesAsync();
        }
    }
}
