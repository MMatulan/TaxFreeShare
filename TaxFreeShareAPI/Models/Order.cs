using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFreeShareAPI.Models;


public class Order
{
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; } 

    [ForeignKey("UserId")]
    public User? User { get; set; } 

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    public decimal TotalAmount { get; set; } 

    public string? Status { get; set; } = "Pending"; 
}