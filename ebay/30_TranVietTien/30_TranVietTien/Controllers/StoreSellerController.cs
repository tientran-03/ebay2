using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;

public class StoreSellerController : Controller
{
    private readonly CloneEbayDbContext _context;

    public StoreSellerController(CloneEbayDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var stores = _context.Stores
            .Include(s => s.Seller)
            .ToList();

        return View(stores);
    }

    [HttpPost]
    public IActionResult Create(string storeName, string description, string bannerUrl, int sellerId)
    {
        var store = new Store
        {
            StoreName = storeName,
            Description = description,
            BannerImageUrl = bannerUrl,
            SellerId = sellerId
        };

        _context.Stores.Add(store);
        _context.SaveChanges();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult Edit(int id, string storeName, string description, string bannerUrl)
    {
        var store = _context.Stores.FirstOrDefault(s => s.Id == id);
        if (store == null)
            return Json(new { success = false });

        store.StoreName = storeName;
        store.Description = description;
        store.BannerImageUrl = bannerUrl;

        _context.SaveChanges();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var store = _context.Stores.FirstOrDefault(s => s.Id == id);
        if (store == null)
            return Json(new { success = false });

        _context.Stores.Remove(store);
        _context.SaveChanges();

        return Json(new { success = true });
    }
}
