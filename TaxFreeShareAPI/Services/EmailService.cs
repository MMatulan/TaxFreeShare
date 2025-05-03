using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TaxFreeShareAPI.Contracts;
using TaxFreeShareAPI.Options;
using TaxFreeShareAPI.Services.Interface;

namespace TaxFreeShareAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public async Task SendEmailAsync(SendEmailRequest sendEmailRequest)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_emailOptions.Email),
                Subject = sendEmailRequest.Subject,
                Body = sendEmailRequest.Body
            };
        
            mailMessage.To.Add(sendEmailRequest.Recipient);
        
            using var smtpClient = new SmtpClient();
            smtpClient.Host = _emailOptions.Host;
            smtpClient.Port = _emailOptions.Port;
            smtpClient.Credentials = new NetworkCredential(
                _emailOptions.Email, _emailOptions.Password);
            smtpClient.EnableSsl = true;
        
            await smtpClient.SendMailAsync(mailMessage);
        
        }
    }
}