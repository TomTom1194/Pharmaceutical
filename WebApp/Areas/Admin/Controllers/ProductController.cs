using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly IProductService _service;
    private readonly IProductCategoryService _categoryService;
    private readonly ITabletSpecificationService _tabletSpecService;
    private readonly ICapsuleSpecificationService _capsuleSpecService;
    private readonly ILiquidFillingSpecificationService _liquidFillingSpecService;
    private readonly IConfiguration _config;

    public ProductController(
        IProductService service,
        IProductCategoryService categoryService,
        ITabletSpecificationService tabletSpecService,
        ICapsuleSpecificationService capsuleSpecService,
        ILiquidFillingSpecificationService liquidFillingSpecService,
        IConfiguration config)
    {
        _service = service;
        _categoryService = categoryService;
        _tabletSpecService = tabletSpecService;
        _capsuleSpecService = capsuleSpecService;
        _liquidFillingSpecService = liquidFillingSpecService;
        _config = config;
    }

    public async Task<IActionResult> Index(int? categoryId, string? keyword, bool? isPublished, int page = 1, int pageSize = 10)
    {
        var allProducts = await _service.GetAll();
        var categories = await _categoryService.GetAll();

        var filtered = allProducts.AsEnumerable();

        if (categoryId.HasValue)
            filtered = filtered.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(keyword))
            filtered = filtered.Where(p => p.ModelName.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        if (isPublished.HasValue)
            filtered = filtered.Where(p => p.IsPublished == isPublished);

        var filteredList = filtered.OrderBy(p => p.ProductId).ToList();
        var totalCount = filteredList.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        var pageItems = filteredList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var tabletSpecs = await _tabletSpecService.GetAll();
        var capsuleSpecs = await _capsuleSpecService.GetAll();
        var liquidFillingSpecs = await _liquidFillingSpecService.GetAll();

        ViewBag.Categories = categories;
        ViewBag.TabletProductIds = tabletSpecs.Select(s => s.ProductId).ToHashSet();
        ViewBag.CapsuleProductIds = capsuleSpecs.Select(s => s.ProductId).ToHashSet();
        ViewBag.LiquidFillingProductIds = liquidFillingSpecs.Select(s => s.ProductId).ToHashSet();
        ViewBag.ApiBaseUrl = _config["ApiBaseUrl"];
        ViewBag.CategoryId = categoryId;
        ViewBag.Keyword = keyword;
        ViewBag.IsPublished = isPublished;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = totalPages;

        return View(pageItems);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _categoryService.GetAll();
        return View(new Product());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAll();
            return View(product);
        }

        await _service.Create(product, mainImage, subImage1, subImage2);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _service.GetById(id);
        if (product == null)
            return NotFound();

        ViewBag.Categories = await _categoryService.GetAll();
        ViewBag.ApiBaseUrl = _config["ApiBaseUrl"];
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAll();
            ViewBag.ApiBaseUrl = _config["ApiBaseUrl"];
            return View(product);
        }

        await _service.Update(id, product, mainImage, subImage1, subImage2);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SetPublished(int id, bool isPublished, string? returnUrl)
    {
        var product = await _service.GetById(id);
        if (product == null)
            return NotFound();

        product.IsPublished = isPublished;
        await _service.Update(id, product, null, null, null);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }
}
