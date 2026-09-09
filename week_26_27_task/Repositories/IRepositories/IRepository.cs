using System.Linq.Expressions;

namespace week_26_27_task.Repositories.IRepositories;
public interface IRepository<T> where T : class
{
        Task<bool> Add(T entity, CancellationToken ct = default);

        bool Update(T entity, CancellationToken ct = default);

        bool Delete(T entity, CancellationToken ct = default);

        Task<bool> CommitAsync(CancellationToken ct = default);

        IQueryable<T> Get(
        Expression<Func<T, bool>>? exprission = null,
        bool tracking = true,
        params Expression<Func<T, object>>[] relations
        );

        T? GetOne(
        Expression<Func<T, bool>>? exprission = null,
        bool tracking = true,
        params Expression<Func<T, object>>[] relations
    );
}
