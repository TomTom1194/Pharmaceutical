using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

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

    private const int PageSize = 9;

    public async Task<IActionResult> Index(int? categoryId, string? keyword, int page = 1)
    {
        var published = (await _service.GetAll()).Where(p => p.IsPublished == true).ToList();

        var filtered = published.AsEnumerable();

        if (categoryId.HasValue)
            filtered = filtered.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(keyword))
            filtered = filtered.Where(p =>
                p.ModelName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                (p.Summary != null && p.Summary.Contains(keyword, StringComparison.OrdinalIgnoreCase)));

        var filteredList = filtered.OrderByDescending(p => p.ProductId).ToList();
        var totalCount = filteredList.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));

        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        var pageItems = filteredList.Skip((page - 1) * PageSize).Take(PageSize).ToList();

        var categories = await _categoryService.GetAll();

        ViewBag.Categories = categories;
        ViewBag.CategoryCounts = categories.ToDictionary(c => c.CategoryId, c => published.Count(p => p.CategoryId == c.CategoryId));
        ViewBag.TotalPublished = published.Count;
        ViewBag.ApiBaseUrl = _config["ApiBaseUrl"];
        ViewBag.CategoryId = categoryId;
        ViewBag.Keyword = keyword;
        ViewBag.Page = page;
        ViewBag.PageSize = PageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = totalPages;

        return View(pageItems);
    }

    public async Task<IActionResult> Favorites()
    {
        var published = (await _service.GetAll()).Where(p => p.IsPublished == true).ToList();

        ViewBag.ApiBaseUrl = _config["ApiBaseUrl"];

        return View(published.OrderByDescending(p => p.ProductId).ToList());
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _service.GetById(id);
        if (product == null || product.IsPublished != true)
            return NotFound();

        var categories = await _categoryService.GetAll();
        ViewBag.Category = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
        ViewBag.ApiBaseUrl = _config["ApiBaseUrl"];

        var specItems = new List<(string Label, string Value)>();

        var tabletSpec = await _tabletSpecService.GetById(id);
        if (tabletSpec != null)
        {
            if (!string.IsNullOrEmpty(tabletSpec.ModelNumber)) specItems.Add(("Model number", tabletSpec.ModelNumber));
            if (tabletSpec.Dies.HasValue) specItems.Add(("Number of punches", tabletSpec.Dies.Value.ToString()));
            if (tabletSpec.MaxPressure.HasValue) specItems.Add(("Maximum pressing force (KN)", tabletSpec.MaxPressure.Value.ToString()));
            if (tabletSpec.MaxDiameterMm.HasValue) specItems.Add(("Maximum tablet diameter (mm)", tabletSpec.MaxDiameterMm.Value.ToString()));
            if (tabletSpec.MaxDepthFillMm.HasValue) specItems.Add(("Maximum fill depth (mm)", tabletSpec.MaxDepthFillMm.Value.ToString()));
            if (tabletSpec.ProductionCapacity.HasValue) specItems.Add(("Production capacity (tablets/h)", tabletSpec.ProductionCapacity.Value.ToString()));
            if (!string.IsNullOrEmpty(tabletSpec.MachineSize)) specItems.Add(("Machine size", tabletSpec.MachineSize));
            if (tabletSpec.NetWeightKg.HasValue) specItems.Add(("Net weight (kg)", tabletSpec.NetWeightKg.Value.ToString()));
        }
        else
        {
            var capsuleSpec = await _capsuleSpecService.GetById(id);
            if (capsuleSpec != null)
            {
                if (!string.IsNullOrEmpty(capsuleSpec.Output)) specItems.Add(("Output", capsuleSpec.Output));
                if (capsuleSpec.CapsuleSizeMm.HasValue) specItems.Add(("Capsule size (mm)", capsuleSpec.CapsuleSizeMm.Value.ToString()));
                if (!string.IsNullOrEmpty(capsuleSpec.MachineDimension)) specItems.Add(("Machine dimension", capsuleSpec.MachineDimension));
                if (capsuleSpec.ShippingWeightKg.HasValue) specItems.Add(("Shipping weight (kg)", capsuleSpec.ShippingWeightKg.Value.ToString()));
            }
            else
            {
                var liquidSpec = await _liquidFillingSpecService.GetById(id);
                if (liquidSpec != null)
                {
                    if (liquidSpec.AirPressure.HasValue) specItems.Add(("Air pressure", liquidSpec.AirPressure.Value.ToString()));
                    if (liquidSpec.AirVolume.HasValue) specItems.Add(("Air volume", liquidSpec.AirVolume.Value.ToString()));
                    if (liquidSpec.FillingSpeed.HasValue) specItems.Add(("Filling speed", liquidSpec.FillingSpeed.Value.ToString()));
                    if (liquidSpec.FillingRangeMl.HasValue) specItems.Add(("Filling range (ml)", liquidSpec.FillingRangeMl.Value.ToString()));
                }
            }
        }

        ViewBag.SpecItems = specItems;

        var allProducts = await _service.GetAll();
        ViewBag.RelatedProducts = allProducts
            .Where(p => p.IsPublished == true && p.CategoryId == product.CategoryId && p.ProductId != product.ProductId)
            .OrderByDescending(p => p.ProductId)
            .Take(12)
            .ToList();

        return View(product);
    }
}
