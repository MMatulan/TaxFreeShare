using System.Net.Http.Json;
using TaxFreeShareFrontend3.Models.DTO;

namespace TaxFreeShareFrontend3.Services;

public class UserService
{
    private readonly HttpClient _http;

    public UserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<UserDto>($"api/users/{id}");
    }
    
}