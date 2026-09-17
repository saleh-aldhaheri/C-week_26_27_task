using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace week_26_27.Service;

public class EmailSender : IEmailSender 
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("salehaldhaheri09@gmail.com", "lmnx axbu krur uhiv")
        };

        var emailMessage = new MailMessage(from: "salehaldhaheri09@gmail.com", to:email, subject: subject, body: htmlMessage)
        {
            IsBodyHtml = true
        };

        return client.SendMailAsync(emailMessage);
    }
}
