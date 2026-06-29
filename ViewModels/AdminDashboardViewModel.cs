using MyFirstNewProject.Models;

namespace MyFirstNewProject.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalRoles { get; set; }
    public Dictionary<string, int> UsersByRole { get; set; } = new();
    public List<ApplicationUser> RecentRegistrations { get; set; } = new();
}