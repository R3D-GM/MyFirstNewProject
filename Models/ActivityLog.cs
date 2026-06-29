namespace MyFirstNewProject.Models;

public class ActivityLog
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? Action { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Severity { get; set; } // Info, Warning, Error
}