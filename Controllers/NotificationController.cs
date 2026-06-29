using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFirstNewProject.Models;
using MyFirstNewProject.Services;

namespace MyFirstNewProject.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly NotificationService _notificationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationController(
        NotificationService notificationService,
        UserManager<ApplicationUser> userManager)
    {
        _notificationService = notificationService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var notifications = await _notificationService.GetUserNotificationsAsync(user.Id, 50);
        return View(notifications);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false });
        }

        await _notificationService.MarkAsReadAsync(id, user.Id);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false });
        }

        await _notificationService.MarkAllAsReadAsync(user.Id);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false });
        }

        await _notificationService.DeleteNotificationAsync(id, user.Id);
        return Json(new { success = true });
    }

    public async Task<IActionResult> GetUnreadCount()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { count = 0 });
        }

        var count = await _notificationService.GetUnreadCountAsync(user.Id);
        return Json(new { count });
    }

    public async Task<IActionResult> GetRecent()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return PartialView("_NotificationDropdown", new List<Notification>());
        }

        var notifications = await _notificationService.GetUserNotificationsAsync(user.Id, 5);
        return PartialView("_NotificationDropdown", notifications);
    }
}