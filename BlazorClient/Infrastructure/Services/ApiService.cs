using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BlazorClient.Infrastructure.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    // GET Request
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"GET request failed for {endpoint}");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, $"JSON deserialization failed for {endpoint}");
            throw;
        }
    }

    // GET Request with Query Parameters
    public async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams)
    {
        try
        {
            var uri = BuildUriWithQuery(endpoint, queryParams);
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"GET request failed for {endpoint}");
            throw;
        }
    }

    // POST Request
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"POST request failed for {endpoint}");
            throw;
        }
    }

    // POST Request (No Response)
    public async Task PostAsync<T>(string endpoint, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"POST request failed for {endpoint}");
            throw;
        }
    }

    // PUT Request
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"PUT request failed for {endpoint}");
            throw;
        }
    }

    // DELETE Request
    public async Task DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"DELETE request failed for {endpoint}");
            throw;
        }
    }

    // DELETE Request with Query Parameters
    public async Task DeleteAsync(string endpoint, Dictionary<string, string> queryParams)
    {
        try
        {
            var uri = BuildUriWithQuery(endpoint, queryParams);
            var response = await _httpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"DELETE request failed for {endpoint}");
            throw;
        }
    }

    // PATCH Request
    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"PATCH request failed for {endpoint}");
            throw;
        }
    }

    // Upload File (Multipart Form Data)
    public async Task<TResponse?> UploadFileAsync<TResponse>(string endpoint, Stream fileStream, string fileName)
    {
        try
        {
            using var formData = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            formData.Add(fileContent, "file", fileName);

            var response = await _httpClient.PostAsync(endpoint, formData);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"File upload failed for {endpoint}");
            throw;
        }
    }

    // Helper method to build URI with query parameters
    private string BuildUriWithQuery(string endpoint, Dictionary<string, string> queryParams)
    {
        var uri = endpoint;
        if (queryParams.Any())
        {
            uri += "?" + string.Join("&", queryParams.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        }

        return uri;
    }

    // Handle API Response with error checking
    public async Task<ApiResponse<T>> HandleApiResponseAsync<T>(Func<Task<HttpResponseMessage>> apiCall)
    {
        try
        {
            var response = await apiCall();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                return ApiResponse<T>.Success(data, "Operation successful");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<T>.Failure($"API Error: {response.StatusCode}", errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API call failed");
            return ApiResponse<T>.Failure("An error occurred during API call", ex.Message);
        }
    }
}

// Generic API Response Wrapper
public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string? ErrorDetails { get; set; }
    public int? StatusCode { get; set; }

    public static ApiResponse<T> Success(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Failure(string message, string? errorDetails = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            ErrorDetails = errorDetails
        };
    }
}