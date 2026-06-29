namespace MyFirstNewProject.Models;

public class Notification
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? Type { get; set; } // Info, Success, Warning, Error
    public string? UserId { get; set; } // null = all users
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}