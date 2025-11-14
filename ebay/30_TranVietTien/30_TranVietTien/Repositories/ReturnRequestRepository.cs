using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _30_TranVietTien.Repositories
{
    public class ReturnRequestRepository : IReturnRequestRepository
    {
        private readonly CloneEbayDbContext _context;
        public ReturnRequestRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReturnRequest request)
        {
            _context.ReturnRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task<ReturnRequest?> GetByIdAsync(int id)
        {
            return await _context.ReturnRequests
                .Include(r => r.Order)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<ReturnRequest>> GetPendingRequestsAsync()
        {
            return await _context.ReturnRequests
                .Where(r => r.Status == "Pending")
                .ToListAsync();
        }

        public async Task<List<ReturnRequest>> GetByUserAsync(int userId)
        {
            return await _context.ReturnRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.Order)
                .ToListAsync();
        }

        public async Task UpdateAsync(ReturnRequest request)
        {
            _context.ReturnRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
