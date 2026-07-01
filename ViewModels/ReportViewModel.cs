using MyFirstNewProject.Models;

namespace MyFirstNewProject.ViewModels;

public class ReportViewModel
{
    public string? ReportType { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Category { get; set; }

    // Customer Report
    public List<Consignee>? CustomerData { get; set; }
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int InactiveCustomers { get; set; }
    public int TotalCompanies { get; set; }
    public int TotalPersons { get; set; }

    // Activity Report
    public List<ActivityLog>? ActivityLogs { get; set; }
    public int TotalActivities { get; set; }

    // User Report
    public List<ApplicationUser>? Users { get; set; }
    public int TotalUsers { get; set; }
}