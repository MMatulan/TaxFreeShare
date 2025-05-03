using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaxFreeShareAPI.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [JsonIgnore] 
    public string PasswordHash { get; set; } = string.Empty;

    [JsonIgnore] 
    public string Role { get; set; } // "Kjøper" eller "Selger"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? VerificationToken { get; set; }  
    public bool IsVerified { get; set; } = false;  
    
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }


}