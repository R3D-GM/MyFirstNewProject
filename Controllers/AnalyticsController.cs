using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstNewProject.Services;
using MyFirstNewProject.ViewModels;

namespace MyFirstNewProject.Controllers;

[Authorize]
public class AnalyticsController : Controller
{
    private readonly ConsigneeService _consigneeService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        ConsigneeService consigneeService,
        ILogger<AnalyticsController> logger)
    {
        _consigneeService = consigneeService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var customers = await _consigneeService.GetConsigneesAsync();
            
            // If no customers, return empty view
            if (customers == null || !customers.Any())
            {
                return View(new AnalyticsViewModel());
            }

            var viewModel = new AnalyticsViewModel
            {
                TotalCustomers = customers.Count,
                ActiveCustomers = customers.Count(c => c.IsActive),
                InactiveCustomers = customers.Count(c => !c.IsActive),
                TotalCompanies = customers.Count(c => !c.IsPerson),
                TotalPersons = customers.Count(c => c.IsPerson),
                Customers = customers
            };

            // ✅ FIXED: Business Type Distribution
            var totalCustomers = customers.Count;
            var businessGroups = customers
                .GroupBy(c => c.BusinessType ?? "Not Specified")
                .Select(g => new BusinessTypeDistribution
                {
                    Type = g.Key,
                    Count = g.Count(),
                    Percentage = totalCustomers > 0 ? Math.Round((double)g.Count() / totalCustomers * 100, 2) : 0
                })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToList();

            viewModel.BusinessTypeDistribution = businessGroups;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading analytics");
            ViewBag.Error = "Unable to load analytics data. Please try again.";
            return View(new AnalyticsViewModel());
        }
    }
}