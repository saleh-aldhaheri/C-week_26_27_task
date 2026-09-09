using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using week_26_27_task.Data.ApplicationDbContext;
using week_26_27_task.Repositories.IRepositories;

namespace week_26_27_task.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;
    private DbSet<T> _db;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _db = _dbContext.Set<T>();
    }

    public async Task<bool> Add(T entity, CancellationToken ct = default)
    {
        try
        {
            await _db.AddAsync(entity, ct);

            return true;
        }
        catch (Exception)
        {
            return false;

        }
    }

    public bool Update(T entity, CancellationToken ct = default)
    {
        try
        {
            _db.Update(entity);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Delete(T entity, CancellationToken ct = default)
    {
        try
        {
            _db.Remove(entity);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CommitAsync(CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public IQueryable<T> Get(
        Expression<Func<T, bool>>? exprission = null,
        bool tracking = true,
        params Expression<Func<T, object>>[] relations
    )
    {
        var query = _db.AsQueryable();

        if (exprission is not null)
        {
            query = query.Where(exprission);
        }

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        foreach (var relation in relations)
        {
            query = query.Include(relation);
        }

        return query;
    }

    public T? GetOne(
        Expression<Func<T, bool>>? exprission = null,
        bool tracking = true,
        params Expression<Func<T, object>>[] relations
    )
    {
        return Get(exprission, tracking, relations).FirstOrDefault();
    }
}
