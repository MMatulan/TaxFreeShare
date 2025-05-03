namespace TaxFreeShareAPI.Models.TransactionsDto;

public class TransactionDetailsDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int TravelerId { get; set; }
    public string TravelerEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal OrderTotalAmount { get; set; }
}