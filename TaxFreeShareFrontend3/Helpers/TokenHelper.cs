using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TaxFreeShareFrontend3.Helpers;

public static class TokenHelper
{
    public static ClaimsPrincipal? GetPrincipalFromToken(string? token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwtToken.Claims);
        return new ClaimsPrincipal(identity);
    }

    public static string? GetEmailFromToken(string? token)
    {
        var principal = GetPrincipalFromToken(token);
        return principal?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static string? GetNameIdentifier(string? token)
    {
        var principal = GetPrincipalFromToken(token);
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}