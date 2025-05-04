namespace TaxFreeShareFrontend3.Models.DTO;

public class CreateOrderDto
{
    public List<UpdateOrderItemDto> OrderItems { get; set; } = new();
}