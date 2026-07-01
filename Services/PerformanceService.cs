using System.Diagnostics;

namespace MyFirstNewProject.Services;

public class PerformanceService
{
    private readonly ILogger<PerformanceService> _logger;

    public PerformanceService(ILogger<PerformanceService> logger)
    {
        _logger = logger;
    }

    public async Task<T> MeasureAsync<T>(string operationName, Func<Task<T>> action)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await action();
            stopwatch.Stop();
            _logger.LogInformation("{Operation} completed in {Elapsed}ms", operationName, stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "{Operation} failed after {Elapsed}ms", operationName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
