using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFreeShareAPI.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order? Order { get; set; }

    [Required]
    public int TravelerId { get; set; } // Reisende som kjøper varen

    [ForeignKey("TravelerId")]
    public User? Traveler { get; set; } // Knytter transaksjonen til en bruker

    [Required]
    public decimal Amount { get; set; } // Beløp i NOK

    [Required]
    public string Status { get; set; } = "Unpaid"; // Betalt/Ubetalt

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}