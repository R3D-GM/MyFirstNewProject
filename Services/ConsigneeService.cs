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

            // ✅ Check if response is successful
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("API returned status code: {StatusCode}", response.StatusCode);
                
                // ✅ Return fallback data instead of throwing
                return GetFallbackData("API Unavailable", $"Status: {(int)response.StatusCode}");
            }
            
            var json = await response.Content.ReadAsStringAsync();
            
            // ✅ Check if response is empty
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogWarning("API returned empty response");
                return GetFallbackData("No Data", "API returned empty response");
            }
            
            var consignees = JsonConvert.DeserializeObject<List<Consignee>>(json);
            
            var result = consignees ?? new List<Consignee>();

            // ✅ Check if deserialization returned empty
            if (!result.Any())
            {
                _logger.LogWarning("API returned empty list");
                return GetFallbackData("No Data", "No customers found in API");
            }

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
            
            // ✅ Return fallback data instead of throwing
            return GetFallbackData("Network Error", "Unable to connect to the server. Please check your internet connection.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error");
            return GetFallbackData("Data Error", "Error reading data from server.");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout");
            return GetFallbackData("Timeout", "The request took too long to complete.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching consignees from API");
            
            // ✅ Return fallback data instead of throwing
            return GetFallbackData("Error", "An error occurred while fetching data. Please try again later.");
        }
    }

    // ============================================
    // ✅ FALLBACK DATA METHOD
    // ============================================
    private List<Consignee> GetFallbackData(string errorType, string errorMessage)
    {
        _logger.LogWarning("Returning fallback data due to: {ErrorType} - {ErrorMessage}", errorType, errorMessage);
        
        return new List<Consignee>
        {
            new Consignee
            {
                Id = -1,
                Code = "ERR_001",
                FirstName = "⚠️ Data Unavailable",
                SecondName = errorType,
                ThirdName = errorMessage,
                IsPerson = false,
                IsActive = false,
                BusinessType = "Error",
                CreatedOn = DateTime.Now,
                LastModified = DateTime.Now
            }
        };
    }

    // ============================================
    // ✅ OPTIMIZED GET WITH PAGINATION
    // ============================================
    public async Task<List<Consignee>> GetConsigneesOptimizedAsync(int page = 1, int pageSize = 100)
    {
        var allCustomers = await GetConsigneesAsync();
        return allCustomers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    // ============================================
    // ✅ GET TOTAL COUNT
    // ============================================
    public async Task<int> GetTotalCountAsync()
    {
        var allCustomers = await GetConsigneesAsync();
        return allCustomers.Count;
    }

    // ============================================
    // ✅ CLEAR CACHE
    // ============================================
    public void ClearCache()
    {
        _cache.Remove(CacheKey);
        _logger.LogInformation("Cache cleared");
    }
}