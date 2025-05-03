using System.ComponentModel.DataAnnotations;

namespace TaxFreeShareAPI.Models.DTOs.OrderDto;

public class UpdateOrderItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}