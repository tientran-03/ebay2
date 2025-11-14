using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;

public class AutoApproveReturnService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AutoApproveReturnService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); 

            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<CloneEbayDbContext>();

            var now = DateTime.Now;

            var list = await _context.ReturnRequests
                .Where(r => r.Status == "Pending" &&
                            r.CreatedAt != null &&
                            EF.Functions.DateDiffDay(r.CreatedAt, now) >= 1)
                .ToListAsync();

            foreach (var req in list)
            {
                req.Status = "Approved";
                req.SellerResponse = "Automatically approved ";
            }

            if (list.Any())
                await _context.SaveChangesAsync();
        }
    }
}
