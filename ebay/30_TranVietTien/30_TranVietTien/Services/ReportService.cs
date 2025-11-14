using _30_TranVietTien.Repositories;
using _30_TranVietTien.ViewModels;

namespace _30_TranVietTien.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;
        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<SalesReportViewModel>> GetWeeklyReportAsync(int sellerId)
        {
            var orders = await _repo.GetOrdersBySellerAsync(sellerId);
            var grouped = orders
                .GroupBy(o => System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    o.OrderDate ?? DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                .Select(g => new SalesReportViewModel
                {
                    Period = $"Tuần {g.Key}",
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalPrice ?? 0)
                })
                .OrderBy(r => r.Period)
                .ToList();
            return grouped;
        }

        public async Task<IEnumerable<SalesReportViewModel>> GetMonthlyReportAsync(int sellerId)
        {
            var orders = await _repo.GetOrdersBySellerAsync(sellerId);
            var grouped = orders
                .GroupBy(o => new { o.OrderDate!.Value.Year, o.OrderDate!.Value.Month })
                .Select(g => new SalesReportViewModel
                {
                    Period = $"{g.Key.Month}/{g.Key.Year}",
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalPrice ?? 0)
                })
                .OrderBy(r => r.Period)
                .ToList();
            return grouped;
        }
    }
}
