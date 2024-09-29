using System.Net.Mail;
using System.Net;

namespace CarDealer.Service
{
    public static class EmailService
    {
        public static async void SendMessage(IConfiguration smtpConfig, MailMessage mailMessage, string recepient)
        {
            var _smtpConfig = smtpConfig.GetSection("Smtp");
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_smtpConfig["Username"], _smtpConfig["Password"]),
                EnableSsl = true,
            };
            mailMessage.To.Add(recepient);
            await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
