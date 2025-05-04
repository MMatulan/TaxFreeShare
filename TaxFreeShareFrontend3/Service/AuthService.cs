using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TaxFreeShareFrontend3.Auth;
using TaxFreeShareFrontend3.Models;

namespace TaxFreeShareFrontend3.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authProvider;

    public AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authProvider = authProvider;
    }

    public async Task<LoginResponse> LoginAsync(LoginModel model)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/users/login", model);

            if (!response.IsSuccessStatusCode)
            {
                return new LoginResponse { Success = false, ErrorMessage = await response.Content.ReadAsStringAsync() };
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result is not null)
            {
                await _localStorage.SetItemAsync("authToken", result.Token);
                await _localStorage.SetItemAsync("userRole", result.Role);
                await _localStorage.SetItemAsync("userId", result.UserId.ToString()); 
                
                ((CustomAuthProvider)_authProvider).NotifyUserAuthentication(result.Token);
                Console.WriteLine("Token sendt til auth provider");
                
                result.Success = true;
                return result;
            }

            return new LoginResponse { Success = false, ErrorMessage = "Tomt svar fra serveren" };
        }
        catch (Exception ex)
        {
            return new LoginResponse { Success = false, ErrorMessage = ex.Message };
        }
    }
}