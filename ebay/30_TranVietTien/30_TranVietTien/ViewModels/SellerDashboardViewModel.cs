namespace _30_TranVietTien.Models.ViewModels
{
    public class SellerDashboardViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Coupon> Coupons { get; set; } = new List<Coupon>();
    }
}
