namespace TaxFreeShareAPI.Models.DTOs.OrderDto;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal TotalAmount { get; set; }
    public List<UpdateOrderItemDto> OrderItems { get; set; } = new();
    
    public int? SellerId { get; set; }

}