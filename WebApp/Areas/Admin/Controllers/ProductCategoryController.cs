using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductCategoryController : Controller
{
    private readonly IProductCategoryService _service;

    public ProductCategoryController(IProductCategoryService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(string? keyword, bool? isActive, int page = 1, int pageSize = 10)
    {
        var categories = (await _service.GetAll()).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(keyword))
            categories = categories.Where(c => c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        if (isActive.HasValue)
            categories = categories.Where(c => c.IsActive == isActive);

        var filteredList = categories.OrderByDescending(c => c.CategoryId).ToList();
        var totalCount = filteredList.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        var pageItems = filteredList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.Keyword = keyword;
        ViewBag.IsActive = isActive;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = totalPages;

        return View(pageItems);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _service.GetById(id);
        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ProductCategory category)
    {
        if (!ModelState.IsValid)
            return View(category);

        await _service.Update(id, category);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
