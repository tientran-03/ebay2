namespace _30_TranVietTien.Models
{
    public class DashboardStatsViewModel
    {
        public decimal RevenueToday { get; set; }
        public decimal RevenueMonth { get; set; }
        public decimal RevenueQuarter { get; set; }

        public int OrdersToday { get; set; }
        public int OrdersMonth { get; set; }
        public int OrdersQuarter { get; set; }
    }
}
