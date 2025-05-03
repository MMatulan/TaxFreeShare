using System.ComponentModel.DataAnnotations;

namespace TaxFreeShareAPI.Models.TransactionsDto;

public class TransactionDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int TravelerId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Unpaid";
    public DateTime CreatedAt { get; set; }
}