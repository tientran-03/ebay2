namespace _30_TranVietTien.ViewModels
{
    public class SalesReportViewModel
    {
        public string Period { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
