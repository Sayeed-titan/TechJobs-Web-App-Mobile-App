using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechJobs.Application.Interfaces.Repositories;
using TechJobs.Infrastructure.Data;

namespace TechJobs.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _ctx;
    private readonly DbSet<T> _db;

    public GenericRepository(AppDbContext ctx)
    {
        _ctx = ctx;
        _db = _ctx.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, params string[] includes)
    {
        IQueryable<T> query = _db;
        foreach (var inc in includes) query = query.Include(inc);

        // Works for entities that have an int "Id" property (our BaseEntity pattern)
        return await query.SingleOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, params string[] includes)
    {
        IQueryable<T> query = _db;
        if (filter != null) query = query.Where(filter);
        foreach (var inc in includes) query = query.Include(inc);
        return await query.ToListAsync();
    }

    public async Task AddAsync(T entity) => await _db.AddAsync(entity);
    public void Update(T entity) => _db.Update(entity);
    public void Delete(T entity) => _db.Remove(entity);
}
