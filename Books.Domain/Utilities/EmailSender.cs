
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Books.Domain.Utilities
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration Configuration { get; }
        private readonly ILogger<EmailSender> _logger;
        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(Configuration["EmailSenderSettings:From"]));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            Send(emailMessage);

            return Task.CompletedTask;
        }
 
        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(Configuration["EmailSenderSettings:SmtpServer"], int.Parse(Configuration["EmailSenderSettings:Port"]), true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(Configuration["EmailSenderSettings:Username"], Configuration["EmailSenderSettings:Password"]);
                client.Send(mailMessage);
            }
            catch
            {
                //log an error message or throw an exception or both.
                _logger.LogError("Error loading external login information during confirmation.");
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }

}
