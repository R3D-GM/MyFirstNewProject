using MyFirstNewProject.Data;
using MyFirstNewProject.Models;

namespace MyFirstNewProject.Services;

public class ActivityLogService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ActivityLogService> _logger;

    public ActivityLogService(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ActivityLogService> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(string action, string details, string severity = "Info")
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userEmail = httpContext?.User?.Identity?.Name ?? "System";
            var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

            var log = new ActivityLog
            {
                UserId = userId,
                UserEmail = userEmail,
                Action = action,
                Details = details,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now,
                Severity = severity
            };

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log activity");
        }
    }
}