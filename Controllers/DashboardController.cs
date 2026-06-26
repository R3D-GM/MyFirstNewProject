using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstNewProject.Services;
using MyFirstNewProject.ViewModels;

namespace MyFirstNewProject.Controllers;

[Authorize] 

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly ConsigneeService _consigneeService;

    public DashboardController(ILogger<DashboardController> logger, ConsigneeService consigneeService)
    {
        _logger = logger;
        _consigneeService = consigneeService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Get all customers from API
            var customers = await _consigneeService.GetConsigneesAsync();
            
            // Calculate metrics
            var viewModel = new DashboardViewModel
            {
                TotalCustomers = customers.Count,
                ActiveCustomers = customers.Count(c => c.IsActive),
                InactiveCustomers = customers.Count(c => !c.IsActive),
                TotalPersons = customers.Count(c => c.IsPerson),
                TotalCompanies = customers.Count(c => !c.IsPerson),
                Customers = customers
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