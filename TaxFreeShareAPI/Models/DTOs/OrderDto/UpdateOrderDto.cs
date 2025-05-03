using System.ComponentModel.DataAnnotations;

namespace TaxFreeShareAPI.Models.DTOs.OrderDto;

public class UpdateOrderDto
{
    [Required]
    public List<UpdateOrderItemDto> OrderItems { get; set; } = new();

    public string? Status { get; set; }
}