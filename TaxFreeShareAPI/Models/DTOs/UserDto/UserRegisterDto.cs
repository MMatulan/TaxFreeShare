using System.ComponentModel.DataAnnotations;

namespace TaxFreeShareAPI.Models;

public class UserRegisterDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } // "Kjøper" eller "Selger"

}