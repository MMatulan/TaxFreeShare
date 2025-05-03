using System.Net.Http.Json;
using TaxFreeShareFrontend3.Models.DTO;

namespace TaxFreeShareFrontend3.Services;

public class OrderService
{
    private readonly HttpClient _http;

    public OrderService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> CreateOrderAsync(CreateOrderDto order)
    {
        var response = await _http.PostAsJsonAsync("api/orders", order);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<OrderDto>> GetOrdersForCurrentUserAsync()
    {
        return await _http.GetFromJsonAsync<List<OrderDto>>("api/orders");
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
    {
        return await _http.GetFromJsonAsync<OrderDto>($"api/orders/{orderId}");
    }

    public async Task<bool> UpdateOrderAsync(int orderId, UpdateOrderDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/orders/update/{orderId}", dto);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        var response = await _http.PutAsJsonAsync($"api/orders/status/{orderId}", new { Status = status });
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> AssignOrderToSellerAsync(int orderId)
    {
        var dto = new OrderDto { Id = orderId }; 
        var response = await _http.PostAsJsonAsync("api/orders/assign", dto);
        return response.IsSuccessStatusCode;
    }


    
}