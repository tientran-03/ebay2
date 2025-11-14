using _30_TranVietTien.ViewModels;

namespace _30_TranVietTien.Services
{
    public interface IReportService
    {
        Task<IEnumerable<SalesReportViewModel>> GetWeeklyReportAsync(int sellerId);
        Task<IEnumerable<SalesReportViewModel>> GetMonthlyReportAsync(int sellerId);
    }
}
