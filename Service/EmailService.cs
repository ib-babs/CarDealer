using System.Net.Mail;
using System.Net;

namespace CarDealer.Service
{
    public static class EmailService
    {
        public static async void SendMessage(MailMessage mailMessage, string recepient)
        {
            var username = Environment.GetEnvironmentVariable("SmtpUsername");
            var password = Environment.GetEnvironmentVariable("SmtpPassword");
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };
            mailMessage.To.Add(recepient);
            await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
