using System.ComponentModel.DataAnnotations;

namespace TaxFreeShareAPI.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }
    
    public string Brand { get; set; } = string.Empty;
    

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}