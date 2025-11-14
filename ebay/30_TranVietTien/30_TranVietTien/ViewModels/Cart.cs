// File: ViewModels/Cart/CartItemVM.cs
namespace _30_TranVietTien.ViewModels.Cart
{
    // Record dùng cho Cart hiển thị ở View
    public record CartItemVM(int ProductId, string Title, decimal Price, int Qty, string? Image);
}
