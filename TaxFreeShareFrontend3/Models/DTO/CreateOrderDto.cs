namespace TaxFreeShareFrontend3.Models.DTO;

public class CreateOrderDto
{
    public List<OrderItemDto> OrderItems { get; set; } = new();
}