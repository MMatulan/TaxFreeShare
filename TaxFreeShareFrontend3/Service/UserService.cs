using System.Net.Http.Json;
using TaxFreeShareFrontend3.Models.DTO;

namespace TaxFreeShareFrontend3.Services;

public class UserService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UserService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("AuthorizedClient");
        return await client.GetFromJsonAsync<UserDto>($"api/users/{id}");
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        var client = _httpClientFactory.CreateClient("AuthorizedClient");
        return await client.GetFromJsonAsync<UserDto>("api/users/me");
    }

    public async Task<bool> UpdateUserAsync(UserDto user)
    {
        var client = _httpClientFactory.CreateClient("AuthorizedClient");
        var response = await client.PutAsJsonAsync("api/users/update", user);
        return response.IsSuccessStatusCode;
    }
}