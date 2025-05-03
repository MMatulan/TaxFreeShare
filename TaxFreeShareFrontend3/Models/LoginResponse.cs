namespace TaxFreeShareFrontend3.Models;

public class LoginResponse
{
    public string Token { get; set; }
    public string Role { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    
    public int UserId { get; set; }

}