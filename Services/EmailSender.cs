using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Glowry.Services
{
public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(
                "mennatullahramadanabdallah@gmail.com",
                "nuhc qesq epls sacr"
            ),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress("mennatullahramadanabdallah@gmail.com"),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mail.To.Add(email);

        return client.SendMailAsync(mail);
    }
}

}