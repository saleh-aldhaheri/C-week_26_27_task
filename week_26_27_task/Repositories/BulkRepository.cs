using week_26_27_task.Data.ApplicationDbContext;

namespace week_26_27_task.Repositories;

public class BulkRepository<T> : Repository<T>, IBulkRepository<T> where T : class
{
    public BulkRepository(ApplicationDbContext dbContext) : base(dbContext)
    { }

    public bool DeleteRange(IEnumerable<T> entites)
    {
        try
        {
            _dbContext.RemoveRange(entites);

            return true; 
        }catch(Exception )
        {
            return false;
        }
    }
}
