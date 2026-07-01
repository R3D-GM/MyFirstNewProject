using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstNewProject.Data;
using MyFirstNewProject.Models;
using MyFirstNewProject.Services;
using MyFirstNewProject.ViewModels;

namespace MyFirstNewProject.Controllers;

[Authorize]
public class ReportController : Controller
{
    private readonly ConsigneeService _consigneeService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ReportController> _logger;

    public ReportController(
        ConsigneeService consigneeService,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<ReportController> logger)
    {
        _consigneeService = consigneeService;
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index(
        string? reportType = "Customer",
        string? startDate = null,
        string? endDate = null,
        string? category = null)
    {
        var viewModel = new ReportViewModel
        {
            ReportType = reportType,
            StartDate = startDate,
            EndDate = endDate,
            Category = category
        };

        try
        {
            if (reportType == "Customer")
            {
                var customers = await _consigneeService.GetConsigneesAsync();
                viewModel.CustomerData = customers;
                viewModel.TotalCustomers = customers.Count;
                viewModel.ActiveCustomers = customers.Count(c => c.IsActive);
                viewModel.InactiveCustomers = customers.Count(c => !c.IsActive);
                viewModel.TotalCompanies = customers.Count(c => !c.IsPerson);
                viewModel.TotalPersons = customers.Count(c => c.IsPerson);
            }
            else if (reportType == "Activity")
            {
                var logs = await _context.ActivityLogs
                    .OrderByDescending(l => l.Timestamp)
                    .Take(200)
                    .ToListAsync();
                viewModel.ActivityLogs = logs;
                viewModel.TotalActivities = logs.Count;
            }
            else if (reportType == "User")
            {
                var users = await _userManager.Users.ToListAsync();
                viewModel.Users = users;
                viewModel.TotalUsers = users.Count;
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            ViewBag.Error = "Unable to generate report. Please try again.";
            return View(viewModel);
        }
    }
}