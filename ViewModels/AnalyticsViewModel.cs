using MyFirstNewProject.Models;

namespace MyFirstNewProject.ViewModels;

public class BusinessTypeDistribution
{
    public string? Type { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class AnalyticsViewModel
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int InactiveCustomers { get; set; }
    public int TotalCompanies { get; set; }
    public int TotalPersons { get; set; }
    public List<Consignee> Customers { get; set; } = new();
    public List<BusinessTypeDistribution> BusinessTypeDistribution { get; set; } = new();
}