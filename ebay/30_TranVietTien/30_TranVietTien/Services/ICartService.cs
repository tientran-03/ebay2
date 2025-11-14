using Microsoft.AspNetCore.Mvc;

namespace _30_TranVietTien.Services
{
    public record CartItemVM(int ProductId, string Title, decimal Price, int Qty, string? Image);

    public interface ICartService
    {
        Task AddAsync(HttpContext ctx, int productId, int qty = 1);
        Task RemoveAsync(HttpContext ctx, int productId);
        Task<List<CartItemVM>> GetAsync(HttpContext ctx);
        Task ClearAsync(HttpContext ctx);
    }
}
