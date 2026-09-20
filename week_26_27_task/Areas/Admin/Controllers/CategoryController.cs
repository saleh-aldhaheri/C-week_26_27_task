using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using week_26_27.Utilities;

namespace week_26_27_task.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.SUPER_ADMIN}")]
public class CategoryController : Controller
{
    ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public IActionResult Index([FromQuery]CategoryWithFilterAndPaginationVM categoriesIndex)
    {
        categoriesIndex = _categoryService.GetCategories(categoriesIndex);

        return View(model: categoriesIndex);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category, CancellationToken ct = default)
    {
        if(ModelState.IsValid is false)
            return View(category); 

        try
        {
           await _categoryService.CreateCategory(category, ct);

        }catch (Exception)
        {
            return BadRequest();
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Category created successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update([FromRoute] int id)
    {
        Category? category = null; 

        try
        {
            category = _categoryService.GetCategory(id);
        }
        catch (Exception)
        {
            return NotFound();
        }

        return View(model: category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Category category, CancellationToken ct = default)
    {
        if(ModelState.IsValid is false)
            return View(model: category);

        try
        {
            await _categoryService.UpdateCategory(category, ct);
        }
        catch(Exception)
        {
            return  BadRequest();
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Category updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete([FromRoute]int id, CancellationToken ct = default)
    {
        try
        {
           await _categoryService.DeleteCategory(id, ct);
        }
        catch(Exception)
        {
            return BadRequest();
        }

        TempData[key: NotificationConstants.SUCCESS_NOTIFICATION] = "Category deleted successfully!";

        return RedirectToAction(nameof(Index));
    }
}
