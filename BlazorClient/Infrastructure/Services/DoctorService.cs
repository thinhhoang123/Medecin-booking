using System.Text.Json;
using BlazorClient.Features.Doctors;
using BlazorClient.Infrastructure.Interfaces;

namespace BlazorClient.Infrastructure.Services;

public class DoctorService : IDoctorService
{
    private readonly ApiService _apiService;
    private readonly ILogger<DoctorService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public DoctorService(ApiService apiService, ILogger<DoctorService> logger)
    {
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
            var response = await _apiService.GetAsync<IEnumerable<DoctorDto>>("api/doctors");
            return response ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctors");
            return [];
        }
    }

}