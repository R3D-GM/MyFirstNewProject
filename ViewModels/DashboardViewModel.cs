using MyFirstNewProject.Models;

namespace MyFirstNewProject.ViewModels;

public class DashboardViewModel
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int InactiveCustomers { get; set; }
    public int TotalPersons { get; set; }
    public int TotalCompanies { get; set; }
    public List<Consignee> Customers { get; set; } = new();
}