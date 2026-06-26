using Microsoft.AspNetCore.Identity;

namespace MyFirstNewProject.Models;

public class ApplicationUser : IdentityUser
{
    // Add custom properties here if needed
    public string? FullName { get; set; }
    public DateTime? CreatedAt { get; set; }
}