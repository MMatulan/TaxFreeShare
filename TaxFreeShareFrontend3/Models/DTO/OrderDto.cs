namespace TaxFreeShareFrontend3.Models.DTO;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<UpdateOrderItemDto> OrderItems { get; set; } = new();
}
