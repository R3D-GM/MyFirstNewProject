using MyFirstNewProject.Models;

namespace MyFirstNewProject.ViewModels;

public class DashboardViewModel
{
    // KPI Metrics
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int InactiveCustomers { get; set; }
    public int TotalPersons { get; set; }
    public int TotalCompanies { get; set; }
    
    // Additional KPIs
    public int NewCustomersThisWeek { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public double ActiveRate { get; set; }
    public double CompanyRate { get; set; }
    
    // Raw data for charts
    public List<Consignee> Customers { get; set; } = new();
    
    // Recent Activity
    public List<ActivityLog> RecentActivities { get; set; } = new();
}