using _30_TranVietTien.Models;

namespace _30_TranVietTien.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
