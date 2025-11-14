using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly CloneEbayDbContext _context;
        public UserRepository(CloneEbayDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
