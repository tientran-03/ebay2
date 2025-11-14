using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Services
{
    public class AddressService : IAddressService
    {
        private readonly CloneEbayDbContext _context;

        public AddressService(CloneEbayDbContext context)
        {
            _context = context;
        }

        // ✅ Lấy danh sách địa chỉ của người dùng
        public async Task<IEnumerable<Address>> GetByUserAsync(int userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();
        }

        // ✅ Lấy 1 địa chỉ cụ thể theo Id
        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // ✅ Thêm mới địa chỉ (và có thể đặt làm mặc định)
        public async Task AddAsync(int userId, Address address, bool setDefault = false)
        {
            address.UserId = userId;

            if (setDefault)
            {
                var oldDefault = await _context.Addresses
                    .Where(a => a.UserId == userId && a.IsDefault == true)
                    .ToListAsync();

                foreach (var addr in oldDefault)
                    addr.IsDefault = false;

                address.IsDefault = true;
            }

            _context.Add(address);
            await _context.SaveChangesAsync();
        }

        // ✅ Đặt địa chỉ làm mặc định
        public async Task SetDefaultAsync(int userId, int addressId)
        {
            var userAddresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            foreach (var addr in userAddresses)
                addr.IsDefault = (addr.Id == addressId);

            await _context.SaveChangesAsync();
        }

        // ✅ Xóa địa chỉ
        public async Task DeleteAsync(int id)
        {
            var addr = await _context.Addresses.FindAsync(id);
            if (addr != null)
            {
                _context.Remove(addr);
                await _context.SaveChangesAsync();
            }
        }
    }
}
