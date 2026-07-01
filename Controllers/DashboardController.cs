using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstNewProject.Data;
using MyFirstNewProject.Services;
using MyFirstNewProject.ViewModels;

namespace MyFirstNewProject.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly ConsigneeService _consigneeService;
    private readonly ApplicationDbContext _context;

    public DashboardController(
        ILogger<DashboardController> logger,
        ConsigneeService consigneeService,
        ApplicationDbContext context)  // ✅ ADD THIS
    {
        _logger = logger;
        _consigneeService = consigneeService;
        _context = context;  // ✅ ADD THIS
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Get all customers from API
            var customers = await _consigneeService.GetConsigneesAsync();
            
            var oneWeekAgo = DateTime.Now.AddDays(-7);
            var oneMonthAgo = DateTime.Now.AddDays(-30);
            
            // Calculate metrics
            var viewModel = new DashboardViewModel
            {
                TotalCustomers = customers.Count,
                ActiveCustomers = customers.Count(c => c.IsActive),
                InactiveCustomers = customers.Count(c => !c.IsActive),
                TotalPersons = customers.Count(c => c.IsPerson),
                TotalCompanies = customers.Count(c => !c.IsPerson),
                Customers = customers,
                
                // Additional KPIs
                NewCustomersThisWeek = customers.Count(c => c.CreatedOn >= oneWeekAgo),
                NewCustomersThisMonth = customers.Count(c => c.CreatedOn >= oneMonthAgo),
                ActiveRate = customers.Count > 0 ? Math.Round((double)customers.Count(c => c.IsActive) / customers.Count * 100) : 0,
                CompanyRate = customers.Count > 0 ? Math.Round((double)customers.Count(c => !c.IsPerson) / customers.Count * 100) : 0,
                
                // Recent Activities
                RecentActivities = await _context.ActivityLogs
                    .OrderByDescending(l => l.Timestamp)
                    .Take(5)
                    .ToListAsync()
            };
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            ViewBag.Error = "Unable to load dashboard data. Please try again.";
            return View(new DashboardViewModel());
        }
    }
}