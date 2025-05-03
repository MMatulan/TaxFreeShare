namespace TaxFreeShareFrontend3.Models.DTO;

public class UpdateOrderDto
{
    public string? Status { get; set; }
    public List<UpdateOrderItemDto> OrderItems { get; set; } = new();
}