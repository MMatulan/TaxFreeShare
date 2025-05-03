using TaxFreeShareAPI.Contracts;

namespace TaxFreeShareAPI.Services.Interface;

public interface IEmailService
{
    Task SendEmailAsync( SendEmailRequest sendEmailRequest );
    
}