using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstNewProject.Data;
using MyFirstNewProject.Models;
using MyFirstNewProject.Services;
using MyFirstNewProject.ViewModels;

namespace MyFirstNewProject.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AdminController> _logger;
    private readonly ActivityLogService _activityLogService;
    private readonly ApplicationDbContext _context;
    private readonly NotificationService _notificationService;
    private readonly PerformanceService _performanceService; // ✅ ADDED

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AdminController> logger,
        ActivityLogService activityLogService,
        ApplicationDbContext context,
        NotificationService notificationService,
        PerformanceService performanceService) // ✅ ADDED
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _activityLogService = activityLogService;
        _context = context;
        _notificationService = notificationService;
        _performanceService = performanceService; // ✅ ADDED
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var roles = await _roleManager.Roles.ToListAsync();
        
        var viewModel = new AdminDashboardViewModel
        {
            TotalUsers = users.Count,
            TotalRoles = roles.Count,
            UsersByRole = new Dictionary<string, int>()
        };

        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            viewModel.UsersByRole[role.Name!] = usersInRole.Count;
        }

        viewModel.RecentRegistrations = users
            .Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value >= DateTime.Now.AddDays(-7))
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .ToList();

        await _activityLogService.LogAsync("Viewed Admin Dashboard", "Admin accessed the dashboard");
        
        return View(viewModel);
    }

    public async Task<IActionResult> Users()
    {
        // ✅ Optimized: Only select needed fields
        var users = await _performanceService.MeasureAsync("GetUsers", async () =>
            await _userManager.Users
                .Select(u => new { u.Id, u.Email, u.FullName, u.LockoutEnd, u.CreatedAt })
                .ToListAsync()
        );
        
        var userRoles = new Dictionary<string, string>();
        var userStatus = new Dictionary<string, bool>();

        foreach (var user in users)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var roles = await _userManager.GetRolesAsync(appUser);
            userRoles[user.Id] = roles.FirstOrDefault() ?? "No Role";
            userStatus[user.Id] = user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.UtcNow;
        }

        ViewBag.UserRoles = userRoles;
        ViewBag.UserStatus = userStatus;
        
        await _activityLogService.LogAsync("Viewed User List", "Admin accessed user management");
        
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleUserStatus(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        bool isLocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;

        if (!isLocked)
        {
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _activityLogService.LogAsync("User Deactivated", $"User {user.Email} was deactivated");
            await _notificationService.AddNotificationAsync(userId, "Account Deactivated", 
                "Your account has been deactivated by an administrator. Please contact support.", "Warning");
        }
        else
        {
            user.LockoutEnd = null;
            await _activityLogService.LogAsync("User Activated", $"User {user.Email} was activated");
            await _notificationService.AddNotificationAsync(userId, "Account Activated", 
                "Your account has been reactivated. You can now login again.", "Success");
        }

        await _userManager.UpdateAsync(user);
        return RedirectToAction("Users");
    }

    public async Task<IActionResult> Roles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleCounts = new Dictionary<string, int>();

        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            roleCounts[role.Name!] = usersInRole.Count;
        }

        ViewBag.RoleCounts = roleCounts;
        
        await _activityLogService.LogAsync("Viewed Role List", "Admin accessed role management");
        
        return View(roles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (!string.IsNullOrEmpty(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                await _activityLogService.LogAsync("Role Created", $"Role '{roleName}' was created");
                TempData["Success"] = $"Role '{roleName}' created successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to create role.";
            }
        }
        return RedirectToAction("Roles");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                await _activityLogService.LogAsync("Role Deleted", $"Role '{role.Name}' was deleted");
                TempData["Success"] = $"Role '{role.Name}' deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete role.";
            }
        }
        return RedirectToAction("Roles");
    }

    public async Task<IActionResult> ActivityLogs()
    {
        var logs = await _context.ActivityLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(100)
            .ToListAsync();
        
        return View(logs);
    }
}