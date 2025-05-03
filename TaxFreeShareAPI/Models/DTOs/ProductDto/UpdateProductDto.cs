namespace TaxFreeShareAPI.Models.ProductDto;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Brand { get; set; } = string.Empty;

}