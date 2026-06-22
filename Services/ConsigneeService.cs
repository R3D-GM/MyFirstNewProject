using Microsoft.Extensions.Caching.Memory;
using MyFirstNewProject.Models;
using Newtonsoft.Json;

namespace MyFirstNewProject.Services;

public class ConsigneeService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConsigneeService> _logger;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "Consignees_All";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(2);

    public ConsigneeService(HttpClient httpClient, IConfiguration configuration, ILogger<ConsigneeService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _cache = cache;
        
        var baseUrl = _configuration["ApiSettings:BaseUrl"];
        if (!string.IsNullOrEmpty(baseUrl))
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
    }

    public async Task<List<Consignee>> GetConsigneesAsync()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(CacheKey, out List<Consignee>? cachedConsignees) && cachedConsignees != null)
        {
            _logger.LogInformation("Returning cached consignees data");
            return cachedConsignees;
        }

        try
        {
            var response = await _httpClient.GetAsync("api/consignee/dynamic?gsltype=28");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var consignees = JsonConvert.DeserializeObject<List<Consignee>>(json);
            
            var result = consignees ?? new List<Consignee>();

            // Store in cache
            _cache.Set(CacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(1)
            });

            _logger.LogInformation("Fetched and cached {Count} consignees from API", result.Count);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error fetching consignees");
            throw new Exception("Unable to connect to the server. Please check your internet connection.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching consignees from API");
            throw new Exception("An error occurred while fetching data. Please try again later.");
        }
    }
}