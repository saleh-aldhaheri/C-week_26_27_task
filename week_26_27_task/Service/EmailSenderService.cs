using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace week_26_27.Service;

public class EmailSenderService : IEmailSender
{
    public Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("salehaldhaheri09@gmail.com", "dtya ulyo iqib gsoi")
        };

        return  client.SendMailAsync(
            new MailMessage(from: "salehaldhaheri09@gmail.com",
                            to: to,
                            subject,
                            htmlMessage
                            )
            {
                IsBodyHtml = true
            });
    }
}
