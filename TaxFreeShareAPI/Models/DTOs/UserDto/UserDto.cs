namespace TaxFreeShareAPI.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } // "Kjøper" eller "Selger"
    public bool IsVerified { get; set; }
}