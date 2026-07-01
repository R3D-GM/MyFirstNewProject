using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstNewProject.Data;
using MyFirstNewProject.Models;

namespace MyFirstNewProject.Controllers;

[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(ApplicationDbContext context, ILogger<SettingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _context.SystemSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new SystemSettings
            {
                CompanyName = "ClientVault",
                CompanyEmail = "info@clientvault.com",
                CompanyPhone = "+251-XXX-XXX",
                CompanyAddress = "Addis Ababa, Ethiopia",
                CompanyWebsite = "www.clientvault.com",
                DefaultTheme = "Dark",
                DefaultPageSize = 10,
                EnableNotifications = true,
                EnableDarkMode = true,
                UpdatedAt = DateTime.Now
            };
            _context.SystemSettings.Add(settings);
            await _context.SaveChangesAsync();
        }
        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(SystemSettings model)
    {
        if (ModelState.IsValid)
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (settings != null)
            {
                settings.CompanyName = model.CompanyName;
                settings.CompanyEmail = model.CompanyEmail;
                settings.CompanyPhone = model.CompanyPhone;
                settings.CompanyAddress = model.CompanyAddress;
                settings.CompanyWebsite = model.CompanyWebsite;
                settings.DefaultTheme = model.DefaultTheme;
                settings.DefaultPageSize = model.DefaultPageSize;
                settings.EnableNotifications = model.EnableNotifications;
                settings.EnableDarkMode = model.EnableDarkMode;
                settings.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Settings updated successfully!";
                return RedirectToAction("Index");
            }
        }
        TempData["Error"] = "Failed to update settings.";
        return RedirectToAction("Index");
    }
}