using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace GraceChapelLibraryWebApp.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(string email, string subject, string message)
        {
            
            var emailMessage = new MimeMessage();
            emailMessage.To.Add(new MailboxAddress(email));
            emailMessage.From.Add(new MailboxAddress("Grace Chapel Library", "noreply@gracechapellibrary.com"));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html")
            {
                Text = message
            };

           

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    //client.Authenticate("", "");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (System.Exception)
            {

                throw;
            }

        }
    }
}