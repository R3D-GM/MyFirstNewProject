namespace MyFirstNewProject.Models;

public class SystemSettings
{
    public int Id { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyLogo { get; set; }
    public string? CompanyEmail { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyAddress { get; set; }
    public string? CompanyWebsite { get; set; }
    public string? DefaultTheme { get; set; }
    public int DefaultPageSize { get; set; }
    public bool EnableNotifications { get; set; }
    public bool EnableDarkMode { get; set; }
    public DateTime UpdatedAt { get; set; }
}