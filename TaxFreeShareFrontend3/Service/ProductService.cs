using System.Net.Http.Json;
using TaxFreeShareFrontend3.Models;

namespace TaxFreeShareFrontend3.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var response = await _http.GetFromJsonAsync<List<ProductDto>>("api/products");
            return response ?? new List<ProductDto>();
        }

    }
}