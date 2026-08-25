using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class QuoteRequestController : Controller
{
    private readonly IQuoteRequestService _service;

    public QuoteRequestController(IQuoteRequestService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(string? status, string? keyword, int page = 1, int pageSize = 10)
    {
        var quotes = (await _service.GetAll()).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(status))
            quotes = quotes.Where(q => q.Status == status);

        if (!string.IsNullOrWhiteSpace(keyword))
            quotes = quotes.Where(q =>
                q.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                q.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        var filteredList = quotes.OrderByDescending(q => q.SubmittedAt).ToList();
        var totalCount = filteredList.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        var pageItems = filteredList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.Status = status;
        ViewBag.Keyword = keyword;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = totalPages;

        return View(pageItems);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var quoteRequest = await _service.GetById(id);
        if (quoteRequest == null)
            return NotFound();

        return View(quoteRequest);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(int id)
    {
        var quoteRequest = await _service.GetById(id);
        if (quoteRequest == null)
            return NotFound();

        quoteRequest.Status = "Done";
        await _service.Update(id, quoteRequest);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var quoteRequest = await _service.GetById(id);
        if (quoteRequest == null)
            return NotFound();

        quoteRequest.Status = "Rejected";
        await _service.Update(id, quoteRequest);
        return RedirectToAction(nameof(Details), new { id });
    }
}
