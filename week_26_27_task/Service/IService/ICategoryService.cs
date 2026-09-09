using System.Linq.Expressions;

namespace week_26_27_task.Service.IService;
public interface ICategoryService
{
     CategoryWithFilterAndPaginationVM GetCategories(CategoryWithFilterAndPaginationVM categoriesIndex);

     Task CreateCategory(Category category, CancellationToken ct);

     Task UpdateCategory(Category category, CancellationToken ct);

     Task DeleteCategory(int id, CancellationToken ct);

     Category GetCategory(int id);
     
     IQueryable<Category> GetAllCategories(Expression<Func<Category, bool>> expression);
}
