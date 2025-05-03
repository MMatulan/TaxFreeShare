using System.ComponentModel.DataAnnotations;

namespace TaxFreeShareAPI.Models.DTOs.OrderDto;


public class CreateOrderDto
{
    [Required]
    public List<UpdateOrderItemDto> OrderItems { get; set; } = new();
}

