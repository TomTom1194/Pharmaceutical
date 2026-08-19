using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Dtos;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ContentPageController : Controller
{
    private readonly ContentPageApiService _api;

    public ContentPageController(ContentPageApiService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var list = await _api.GetAll();
        return View(list);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _api.GetById(id);
        if (dto is null) return NotFound();

        var model = new ContentPageFormModel
        {
            PageId = dto.PageId,
            Slug = dto.Slug,
            Title = dto.Title ?? "",
            Body = dto.Body,
            BannerImageUrl = dto.BannerImageUrl,
            Status = dto.Status ?? "Draft"
        };

        if (dto.Slug == "home")
        {
            if (!string.IsNullOrEmpty(dto.Body))
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Deserialize<HomePageContentModel>(dto.Body);
                    if (json != null)
                    {
                        model.HeroTitle = json.HeroTitle;
                        model.HeroSubtitle = json.HeroSubtitle;
                        model.AboutTitle = json.AboutTitle;
                        model.AboutDescription = json.AboutDescription;
                        model.ExistingSliderImages = json.SliderImages ?? new();
                    }
                }
                catch { } // Ignore JSON parsing errors
            }
            return View("EditHome", model);
        }
        else if (dto.Slug == "careers")
        {
            if (!string.IsNullOrEmpty(dto.Body))
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Deserialize<HomePageContentModel>(dto.Body);
                    if (json != null)
                    {
                        model.HeroTitle = json.HeroTitle;
                        model.HeroSubtitle = json.HeroSubtitle;
                        model.AboutTitle = json.AboutTitle;
                        model.AboutDescription = json.AboutDescription;
                        model.JobOpenings = json.JobOpenings ?? new();
                    }
                }
                catch { } // Ignore JSON parsing errors
            }
            return View("EditCareers", model);
        }
        else if (dto.Slug == "quote")
        {
            if (!string.IsNullOrEmpty(dto.Body))
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Deserialize<HomePageContentModel>(dto.Body);
                    if (json != null)
                    {
                        model.HeroTitle = json.HeroTitle;
                        model.HeroSubtitle = json.HeroSubtitle;
                        model.QuoteEmail = json.QuoteEmail;
                        model.QuotePhone = json.QuotePhone;
                    }
                }
                catch { } // Ignore JSON parsing errors
            }
            return View("EditQuote", model);
        }
        else if (dto.Slug == "about-us")
        {
            if (!string.IsNullOrEmpty(dto.Body))
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Deserialize<HomePageContentModel>(dto.Body);
                    if (json != null)
                    {
                        model.AboutTitle = json.AboutTitle;
                        model.AboutDescription = json.AboutDescription;
                        model.CoreValues = json.CoreValues ?? new();
                        
                        model.Body = json.AboutDescription;
                    }
                }
                catch 
                { 
                    
                    model.Body = dto.Body;
                }
            }
            return View("EditAboutUs", model);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ContentPageFormModel form)
    {
        if (form.Slug == "home" && string.IsNullOrEmpty(form.Title))
        {
            form.Title = "Homepage";
            ModelState.Remove("Title");
        }
        else if (form.Slug == "careers" && string.IsNullOrEmpty(form.Title))
        {
            form.Title = "Careers";
            ModelState.Remove("Title");
        }

        if (!ModelState.IsValid) 
        {
            if (form.Slug == "home") return View("EditHome", form);
            if (form.Slug == "careers") return View("EditCareers", form);
            if (form.Slug == "quote") return View("EditQuote", form);
            if (form.Slug == "about-us") return View("EditAboutUs", form);
            return View(form);
        }

        
        if (form.BannerImageUpload != null && form.BannerImageUpload.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + form.BannerImageUpload.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await form.BannerImageUpload.CopyToAsync(fileStream);
            }

            form.BannerImageUrl = "/images/uploads/" + uniqueFileName;
        }

        if (form.Slug == "home")
        {
            var finalSliderImages = new List<string>(form.ExistingSliderImages ?? new List<string>());

            if (form.SliderImageUploads != null && form.SliderImageUploads.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in form.SliderImageUploads)
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        finalSliderImages.Add("/images/uploads/" + uniqueFileName);
                    }
                }
            }

            var jsonModel = new HomePageContentModel
            {
                HeroTitle = form.HeroTitle,
                HeroSubtitle = form.HeroSubtitle,
                AboutTitle = form.AboutTitle,
                AboutDescription = form.AboutDescription,
                SliderImages = finalSliderImages
            };
            form.Body = System.Text.Json.JsonSerializer.Serialize(jsonModel);
        }
        else if (form.Slug == "careers")
        {
            var jsonModel = new HomePageContentModel
            {
                HeroTitle = form.HeroTitle,
                HeroSubtitle = form.HeroSubtitle,
                AboutTitle = form.AboutTitle,
                AboutDescription = form.AboutDescription,
                JobOpenings = form.JobOpenings ?? new()
            };
            form.Body = System.Text.Json.JsonSerializer.Serialize(jsonModel);
        }
        else if (form.Slug == "quote")
        {
            var jsonModel = new HomePageContentModel
            {
                HeroTitle = form.HeroTitle,
                HeroSubtitle = form.HeroSubtitle,
                QuoteEmail = form.QuoteEmail,
                QuotePhone = form.QuotePhone
            };
            form.Body = System.Text.Json.JsonSerializer.Serialize(jsonModel);
        }
        else if (form.Slug == "about-us")
        {
            var jsonModel = new HomePageContentModel
            {
                AboutTitle = form.AboutTitle,
                AboutDescription = form.Body, 
                CoreValues = form.CoreValues ?? new()
            };
            form.Body = System.Text.Json.JsonSerializer.Serialize(jsonModel);
        }

        var dto = new ContentPageDto
        {
            Title = form.Title,
            Body = form.Body,
            BannerImageUrl = form.BannerImageUrl,
            Status = form.Status
        };

        var result = await _api.Update(form.PageId, dto);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Page content updated successfully.";
            return RedirectToAction(nameof(Index), new { previewSlug = form.Slug });
        }

        ModelState.AddModelError("", $"Failed to update page content via API. Details: {result.Error}");
        if (form.Slug == "home") return View("EditHome", form);
        if (form.Slug == "careers") return View("EditCareers", form);
        if (form.Slug == "quote") return View("EditQuote", form);
        if (form.Slug == "about-us") return View("EditAboutUs", form);
        return View(form);
    }
}
