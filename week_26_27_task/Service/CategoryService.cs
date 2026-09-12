using System.Linq.Expressions;
using week_26_27.Repositories.IRepositories;

namespace week_26_27_task.Service;
public class CategoryService : ICategoryService
{
    private IUnitOfWork _unitOfWork;
    private IPagination _pagination;

    public CategoryService(IUnitOfWork unitOfWork, IPagination pagination)
    {
        _unitOfWork = unitOfWork;
        _pagination = pagination;
    }

    public CategoryWithFilterAndPaginationVM GetCategories(CategoryWithFilterAndPaginationVM categoriesIndex)
    {
        var categories = _unitOfWork.categryRepository.Get();

        if (categoriesIndex.Search is not null)
        {
            categories = categories.Where(
                e => e.Name.ToLower().Contains(categoriesIndex.Search.ToLower()));
        }

        if (categoriesIndex.Status is not null)
        {
            categories = categories.Where(e => e.Status == categoriesIndex.Status);
        }

        int skip = (categoriesIndex.Pagination.Page - 1) * categoriesIndex.Pagination.PageSize;

        categoriesIndex.Categories = categories.Skip(skip).Take(categoriesIndex.Pagination.PageSize);

        categoriesIndex.Pagination = _pagination
            .Paginate(
            categories.Count(),
            categoriesIndex.Pagination.PageSize,
            categoriesIndex.Pagination.Page
            );

        return categoriesIndex;
    }

    public async Task CreateCategory(Category category, CancellationToken ct)
    {
        if (!await _unitOfWork.categryRepository.Add(entity: category, ct))
                throw new Exception();

        await _unitOfWork.categryRepository.CommitAsync(ct);
    }

    public  async Task UpdateCategory(Category category, CancellationToken ct)
    {
        if (!_unitOfWork.categryRepository.Update(category))
            throw new Exception(); 

        await _unitOfWork.categryRepository.CommitAsync(ct);
    }

    public async Task DeleteCategory(int id, CancellationToken ct)
    {
        var category = GetCategory(id); 


        if (!_unitOfWork.categryRepository.Delete(category))
            throw new Exception();

        await _unitOfWork.categryRepository.CommitAsync(ct);
    }

    public Category GetCategory(int id)
    {
        var category = _unitOfWork.categryRepository.GetOne(exprission: e => e.Id == id, false);

        if (category is null)
            throw new Exception();

        return category;
    }
    
    public IQueryable<Category> GetAllCategories(Expression<Func<Category,bool>> expression)
    {
        return _unitOfWork.categryRepository.Get(expression);
    }
}
