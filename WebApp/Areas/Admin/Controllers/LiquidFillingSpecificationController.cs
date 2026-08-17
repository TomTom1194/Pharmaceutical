using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class LiquidFillingSpecificationController : Controller
{
    private readonly ILiquidFillingSpecificationService _service;
    private readonly IProductService _productService;

    public LiquidFillingSpecificationController(ILiquidFillingSpecificationService service, IProductService productService)
    {
        _service = service;
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int productId)
    {
        var product = await _productService.GetById(productId);
        if (product == null)
            return NotFound();

        ViewBag.Product = product;
        return View(new LiquidFillingSpecification { ProductId = productId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(LiquidFillingSpecification spec)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Product = await _productService.GetById(spec.ProductId);
            return View(spec);
        }

        await _service.Create(spec);
        return RedirectToAction("Index", "Product");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var spec = await _service.GetById(id);
        if (spec == null)
            return NotFound();

        ViewBag.Product = await _productService.GetById(id);
        return View(spec);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, LiquidFillingSpecification spec)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Product = await _productService.GetById(id);
            return View(spec);
        }

        await _service.Update(id, spec);
        return RedirectToAction("Index", "Product");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return RedirectToAction("Index", "Product");
    }
}
