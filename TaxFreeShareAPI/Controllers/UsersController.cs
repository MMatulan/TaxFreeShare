using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaxFreeShareAPI.Contracts;
using TaxFreeShareAPI.Data;
using TaxFreeShareAPI.Models;
using TaxFreeShareAPI.Services.Interface;

namespace TaxFreeShareAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly TaxFreeDbContext _context;
    private readonly IConfiguration _configuration; 
    private readonly ILogger<UsersController> _logger;


    public UsersController(TaxFreeDbContext context, IConfiguration configuration, ILogger<UsersController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(
        [FromBody] UserRegisterDto userDto,
        [FromServices] IEmailService mailService) 
    {
        _logger.LogInformation($" Forsøker å registrere en ny bruker med e-post: {userDto.Email}");

        if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
        {
            _logger.LogWarning($"Registreringsforsøk med allerede registrert e-post: {userDto.Email}");
            return BadRequest("Email already in use.");
        }
        var user = new User
        {
            Name = userDto.Name,
            Email = userDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password), // Hasher passordet
            Role = userDto.Role == "Selger" ? "Selger" : "Kjøper",
            CreatedAt = DateTime.UtcNow,
            VerificationToken = Guid.NewGuid().ToString()
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Bruker {userDto.Email} registrert. Generert verifikasjonstoken: {user.VerificationToken}");

        // Generer bekreftelseslink
        var confirmLink = $"http://localhost:5143/api/users/confirm-email?token={user.VerificationToken}";
        var emailBody = $@"
        <h2>Velkommen til TaxFreeShare!</h2>
        <p>For å aktivere kontoen din, vennligst bekreft e-postadressen din ved å klikke på lenken nedenfor:</p>
        <p><a href='{confirmLink}' style='background-color:#4CAF50; color:white; padding:10px 20px; text-decoration:none; border-radius:5px;'>Trykk her</a></p>
        <p>Hvis du ikke opprettet denne kontoen, kan du se bort fra denne e-posten.</p>
        <p>Hilsen,<br/>TaxFreeShare Team</p>
            ";
        
        // Konstruer e-postforespørselen ved å sende parametere direkte til konstruktøren
        var sendEmailRequest = new SendEmailRequest(
            user.Email,
            "Bekreft e-postadresse",
            emailBody
        );

        // Send e-posten via mailService
        await mailService.SendEmailAsync(sendEmailRequest);
        _logger.LogInformation($"E-postbekreftelse sendt til {userDto.Email}.");
        
        return Ok("Vennligst sjekk e-posten din for å bekrefte kontoen din.");
    }

    // GET: api/users/confirm-email
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        _logger.LogInformation($"Forsøker å bekrefte e-post med token: {token}");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
        if (user == null) 
        {
            _logger.LogWarning("Ugyldig eller utløpt token for e-postbekreftelse.");
            return BadRequest("Ugyldig eller utløpt token.");
        }
        user.IsVerified = true;
        //user.VerificationToken = null;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Bruker {user.Email} har bekreftet e-posten sin.");
        return Ok("E-postadressen din er nå bekreftet!");
    }

    // POST: api/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginRequest)
    {
        _logger.LogInformation($"Innloggingsforsøk for e-post: {loginRequest.Email}");

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginRequest.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            _logger.LogWarning($"Mislykket innloggingsforsøk for e-post: {loginRequest.Email}");
            return Unauthorized("Invalid email or password.");
        }

        if (!user.IsVerified)
        {
            _logger.LogWarning($"Innlogging blokkert. Bruker {user.Email} har ikke bekreftet e-post.");
            return BadRequest("E-post må bekreftes før innlogging.");
        }

        var token = GenerateJwtToken(user);
        _logger.LogInformation($"Bruker {user.Email} logget inn.");

        return Ok(new { Token = token, role = user.Role, UserId = user.Id });
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new Exception("Missing JWT Key")
        ));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) 
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        _logger.LogInformation($"Henter bruker med ID: {id}");

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            _logger.LogWarning($"Bruker med ID {id} ikke funnet.");
            return NotFound();
        }
        _logger.LogInformation($"Bruker {user.Email} funnet.");
        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            IsVerified = user.IsVerified
        });    }
    
    // POST: api/users/forgot-password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDto request,
        [FromServices] IEmailService mailService) 
    {
        _logger.LogInformation($"Forsøker å tilbakestille passord for e-post: {request.Email}");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) 
        {
            _logger.LogWarning($"Tilbakestillingsforsøk for ikke-eksisterende e-post: {request.Email}");
            return BadRequest("E-post ikke funnet.");
        }
        var resetToken = Guid.NewGuid().ToString();
        user.PasswordResetToken = resetToken;
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _context.SaveChangesAsync();

        var resetLink = $"http://localhost:5200/tilbakestill-passord?token={resetToken}";
        var emailBody = $"Klikk på lenken for å tilbakestille passordet ditt: <a href='{resetLink}'>Tilbakestill passord</a>";

        await mailService.SendEmailAsync(new SendEmailRequest(user.Email, "Tilbakestill passord", emailBody));
        
        _logger.LogInformation($"Tilbakestillingslenke sendt til {user.Email}");
        return Ok("Sjekk e-posten din for å tilbakestille passordet.");
    }


    // POST: api/users/reset-password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        _logger.LogInformation($"Forsøker å tilbakestille passord med token: {request.Token}");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token && u.ResetTokenExpiry > DateTime.UtcNow);
        if (user == null) 
        {
            _logger.LogWarning("Ugyldig eller utløpt token for passordtilbakestilling.");
            return BadRequest("Ugyldig eller utløpt token.");
        }
        
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.ResetTokenExpiry = null;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Passord tilbakestilt for bruker {user.Email}");
        return Ok("Passordet ditt er tilbakestilt!");
    }
}