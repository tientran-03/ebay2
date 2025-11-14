using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly CloneEbayDbContext _ctx;
    protected readonly DbSet<T> _set;
    public GenericRepository(CloneEbayDbContext ctx) { _ctx = ctx; _set = ctx.Set<T>(); }

    public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);
    public IQueryable<T> Query() => _set.AsQueryable();
    public async Task AddAsync(T e) { await _set.AddAsync(e); }
    public async Task AddRangeAsync(IEnumerable<T> es) { await _set.AddRangeAsync(es); }
    public void Update(T e) { _set.Update(e); }
    public void Remove(T e) { _set.Remove(e); }
    public Task<int> SaveAsync() => _ctx.SaveChangesAsync();
}
