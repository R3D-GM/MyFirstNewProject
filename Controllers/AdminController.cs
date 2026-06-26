using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstNewProject.Models;

namespace MyFirstNewProject.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    // ============================================
    // ADMIN DASHBOARD
    // ============================================
    public IActionResult Index()
    {
        return View();
    }

    // ============================================
    // USER MANAGEMENT
    // ============================================
    public async Task<IActionResult> Users()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new Dictionary<string, string>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles[user.Id] = roles.FirstOrDefault() ?? "No Role";
        }

        ViewBag.UserRoles = userRoles;
        return View(users);
    }

    // ============================================
    // ROLE MANAGEMENT
    // ============================================
    public async Task<IActionResult> Roles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }
}