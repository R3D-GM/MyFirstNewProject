using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstNewProject.Services;
using MyFirstNewProject.Models;
using System.Diagnostics;

namespace MyFirstNewProject.Controllers;
[Authorize]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ConsigneeService _consigneeService;

    public HomeController(ILogger<HomeController> logger, ConsigneeService consigneeService)
    {
        _logger = logger;
        _consigneeService = consigneeService;
    }

    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "searchTerm", "sortBy", "sortDirection", "type", "status", "page", "pageSize" })]
    public async Task<IActionResult> Index(
        string? searchTerm = null,
        string? sortBy = null,
        string? sortDirection = null,
        string? type = null,
        string? status = null,
        int page = 1,
        int pageSize = 10)
    {
        try
        {
            // Get all data
            var allData = await _consigneeService.GetConsigneesAsync();

            // -------------------------
            // SEARCH
            // -------------------------
            if (!string.IsNullOrEmpty(searchTerm))
            {
                allData = allData.Where(c =>
                    (c.FirstName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Code?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.SecondName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.ThirdName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            // -------------------------
            // FILTER BY TYPE
            // -------------------------
            if (!string.IsNullOrEmpty(type))
            {
                if (string.Equals(type, "person", StringComparison.OrdinalIgnoreCase))
                {
                    allData = allData.Where(c => c.IsPerson).ToList();
                }
                else if (string.Equals(type, "company", StringComparison.OrdinalIgnoreCase))
                {
                    allData = allData.Where(c => !c.IsPerson).ToList();
                }
            }

            // -------------------------
            // FILTER BY STATUS
            // -------------------------
            if (!string.IsNullOrEmpty(status))
            {
                if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
                {
                    allData = allData.Where(c => c.IsActive).ToList();
                }
                else if (string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
                {
                    allData = allData.Where(c => !c.IsActive).ToList();
                }
            }

            // -------------------------
            // SORTING
            // -------------------------
            sortDirection = sortDirection ?? "asc";

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLowerInvariant())
                {
                    case "id":
                        allData = sortDirection == "asc"
                            ? allData.OrderBy(c => c.Id).ToList()
                            : allData.OrderByDescending(c => c.Id).ToList();
                        break;

                    case "name":
                        allData = sortDirection == "asc"
                            ? allData.OrderBy(c => c.FirstName).ToList()
                            : allData.OrderByDescending(c => c.FirstName).ToList();
                        break;

                    case "code":
                        allData = sortDirection == "asc"
                            ? allData.OrderBy(c => c.Code).ToList()
                            : allData.OrderByDescending(c => c.Code).ToList();
                        break;

                    default:
                        allData = allData.OrderBy(c => c.Id).ToList();
                        break;
                }
            }

            // -------------------------
            // PAGINATION
            // -------------------------
            int totalRecords = allData.Count;
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var pagedData = allData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // -------------------------
            // SEND TO VIEW
            // -------------------------
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.Type = type;
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;

            return View(pagedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Index");
            ViewBag.Error = "Unable to load data. Please try again.";
            return View(new List<Consignee>());
        }
    }

    // ============================================================
    // ✅ ADD THIS DETAILS METHOD (place it here)
    // ============================================================
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var allData = await _consigneeService.GetConsigneesAsync();
            var customer = allData.FirstOrDefault(c => c.Id == id);
            
            if (customer == null)
            {
                ViewBag.Error = "Customer not found.";
                return View();
            }
            
            return View(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching details for ID: {id}");
            ViewBag.Error = "Unable to load customer details.";
            return View();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}