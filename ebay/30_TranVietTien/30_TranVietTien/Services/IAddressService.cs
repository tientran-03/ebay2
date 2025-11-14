using _30_TranVietTien.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _30_TranVietTien.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetByUserAsync(int userId);
        Task<Address?> GetByIdAsync(int id);
        Task AddAsync(int userId, Address address, bool setDefault = false);
        Task SetDefaultAsync(int userId, int addressId);
        Task DeleteAsync(int id);
    }
}
