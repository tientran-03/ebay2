using _30_TranVietTien.Models;
using Microsoft.AspNetCore.Mvc;

namespace _30_TranVietTien.Services
{
    public class CartService : ICartService
    {
        private readonly CloneEbayDbContext _ctx;
        const string KEY = "CART";
        public CartService(CloneEbayDbContext ctx) { _ctx = ctx; }

        public async Task AddAsync(HttpContext ctx, int productId, int qty = 1)
        {
            var cart = await GetAsync(ctx);
            var p = await _ctx.Products.FindAsync(productId);
            if (p == null) return;
            var ex = cart.FirstOrDefault(x => x.ProductId == productId);
            if (ex == null) cart.Add(new CartItemVM(p.Id, p.Title!, p.Price ?? 0, qty, p.Images));
            else cart[cart.IndexOf(ex)] = ex with { Qty = ex.Qty + qty };
            ctx.Session.SetString(KEY, System.Text.Json.JsonSerializer.Serialize(cart));
        }

        public async Task<List<CartItemVM>> GetAsync(HttpContext ctx)
        {
            var s = ctx.Session.GetString(KEY);
            if (string.IsNullOrEmpty(s)) return new();
            return System.Text.Json.JsonSerializer.Deserialize<List<CartItemVM>>(s)!;
        }
        public Task RemoveAsync(HttpContext ctx, int id)
        {
            var cart = GetAsync(ctx).Result;
            cart.RemoveAll(x => x.ProductId == id);
            ctx.Session.SetString(KEY, System.Text.Json.JsonSerializer.Serialize(cart));
            return Task.CompletedTask;
        }
        public Task ClearAsync(HttpContext ctx) { ctx.Session.Remove(KEY); return Task.CompletedTask; }
    }
}
