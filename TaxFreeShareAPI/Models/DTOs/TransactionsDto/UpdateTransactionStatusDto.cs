using System.ComponentModel.DataAnnotations;

namespace TaxFreeShareAPI.Models.TransactionsDto;

public class UpdateTransactionStatusDto
{
    [Required]
    [RegularExpression("^(Paid|Unpaid)$", ErrorMessage = "Status må være enten 'Paid' eller 'Unpaid'.")]
    public string Status { get; set; } = "Unpaid";
    
}