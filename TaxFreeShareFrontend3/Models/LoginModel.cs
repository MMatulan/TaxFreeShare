using System.ComponentModel.DataAnnotations;

namespace TaxFreeShareFrontend3.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
