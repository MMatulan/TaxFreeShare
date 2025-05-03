using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using TaxFreeShareFrontend3;
using TaxFreeShareFrontend3.Auth;
using TaxFreeShareFrontend3.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Local Storage
builder.Services.AddBlazoredLocalStorage();

// Authentication & Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();

// Custom handler for å legge til token
builder.Services.AddScoped<AuthorizationMessageHandler>();

// HTTP-klient med token
builder.Services.AddHttpClient("AuthorizedClient", client =>
    {
        client.BaseAddress = new Uri("http://localhost:5143/");
    })
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

// Registrer tjenester som bruker HTTP-klienten
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("AuthorizedClient");
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddScoped<OrderService>();


await builder.Build().RunAsync();