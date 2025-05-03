using System.ComponentModel.DataAnnotations;

namespace TaxFreeShareAPI.Models.TransactionsDto;

public class CreateTransactionDto
{
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int TravelerId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
}