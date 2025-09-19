
using System.Net.Http.Json;
using System.Net.Http;

public class ApiClient
{
    private readonly HttpClient _http = new() { BaseAddress = new Uri("https://localhost:7155/") };
    private string? _token; 

    public void SetToken(string? token)
    {
        _token = token;
        _http.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public Task<HttpResponseMessage> GetAsync(string url) => _http.GetAsync(url);
    public Task<HttpResponseMessage> PostAsync<T>(string url, T body) => _http.PostAsJsonAsync(url, body);
    public Task<HttpResponseMessage> PostAsync(string url, HttpContent content) => _http.PostAsync(url, content);
    public Task<HttpResponseMessage> PutAsync<T>(string url, T body) => _http.PutAsJsonAsync(url, body);
    public Task<HttpResponseMessage> DeleteAsync(string url) => _http.DeleteAsync(url);
}
