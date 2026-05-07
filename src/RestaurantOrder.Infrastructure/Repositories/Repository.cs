using Microsoft.EntityFrameworkCore;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Infrastructure.Persistence;

namespace RestaurantOrder.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly RestaurantDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(RestaurantDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await DbSet.FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await DbSet.AddAsync(entity, ct);
        await Context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is not null)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync(ct);
        }
    }
}
