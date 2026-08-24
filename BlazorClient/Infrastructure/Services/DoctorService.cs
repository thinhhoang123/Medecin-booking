using System.Text.Json;
using BlazorClient.Features.Doctors;
using BlazorClient.Infrastructure.Interfaces;

namespace BlazorClient.Infrastructure.Services;

public class DoctorService : IDoctorService
{
    private readonly HttpClient _httpClient;
    private readonly ApiService _apiService;
    private readonly ILogger<DoctorService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public DoctorService(HttpClient httpClient, ApiService apiService, ILogger<DoctorService> logger)
    {
        _httpClient = httpClient;
        _apiService = apiService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/doctors");
                
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<DoctorDto>>(json, _jsonOptions) 
                       ?? Enumerable.Empty<DoctorDto>();
            }
            else
            {
                _logger.LogError($"API Error: {response.StatusCode}");
                return Enumerable.Empty<DoctorDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctors");
            return Enumerable.Empty<DoctorDto>();
        }
    }

}