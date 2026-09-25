namespace week_26_27_task.Repositories.IRepositories;
public interface IBulkRepository<T>: IRepository<T> where T : class
{
    bool DeleteRange(IEnumerable<T> entities);
    Task<bool> AddRange(IEnumerable<T> entities);
}
