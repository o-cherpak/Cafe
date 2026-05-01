using CafeApi.Data;
using CafeApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeApi.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly CafeDbContext Db;
    private readonly DbSet<T> _dbSet;

    protected Repository(CafeDbContext db)
    {
        Db = db;
        _dbSet = Db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Db.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}